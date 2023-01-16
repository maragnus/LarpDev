import {authRestService, larpMw5eService, larpRestService, userRestService} from "./LarpService";
import {
    ConfirmLoginRequest,
    InitiateLoginRequest,
    LogoutRequest,
    ValidateSessionRequest,
    ValidationResponseCode
} from "./Protos/larp/authorization_pb";
import {Account} from "./Protos/larp/accounts_pb";
import {GameState} from "./Protos/larp/mw5e/fifthedition_pb";
import {CachedGameState} from "./CachedGameState";
import {
    AccountResponse,
    EventListRequest,
    EventRequest,
    EventRsvpRequest,
    UpdateProfileRequest
} from "./Protos/larp/services_pb";
import {Event, EventRsvp} from "./Protos/larp/events_pb";
import {Character} from "./Protos/larp/mw5e/character_pb";

export interface SessionCallback {
    callback: (() => void);
    subscription: number;
}

export enum LoginStatus {
    Success,
    Blocked,
    Failed
}

export enum ConfirmStatus {
    Success,
    AlreadyUsed,
    Expired,
    Invalid
}

export class SessionService {
    private _callbacks: SessionCallback[] = [];
    private _nextSubscriptionId: number = 0;
    private _email?: string;
    private _mw5eGameState = new CachedGameState<GameState.AsObject>("mw5e");
    private _account: Account.AsObject = new Account().toObject();

    private static readonly SessionIdKey = "MWL_SESSION_ID";

    constructor() {
        const sessionId = localStorage.getItem(SessionService.SessionIdKey) ?? "";
        larpRestService.sessionId = sessionId ?? "";

        this.startIntervals();
    }

    getEmail(): string | undefined {
        return this._email;
    }

    startIntervals() {
        // Make sure that all sessions across tabs identify if we are signed out
        setInterval(function () {
            let sessionId: string | undefined = localStorage.getItem(SessionService.SessionIdKey) ?? "";
            if (sessionId === "") sessionId = undefined;

            if (sessionId !== larpRestService.sessionId)
                sessionService.updateState(sessionId);
        }, 1000);

        setTimeout(async function () {
            await sessionService.validateSession();
        }, 1);

        setInterval(async function() {
            await sessionService.validateSession();
        }, 60 * 1000);
    }

    isAuthenticated(): boolean {
        return larpRestService !== undefined && larpRestService.sessionId !== "" && larpRestService.sessionId !== "bypass";
    }

    updateState(sessionId?: string): void {
        larpRestService.sessionId = sessionId ?? "";
        localStorage.setItem(SessionService.SessionIdKey, sessionId ?? "");
        this.notifySubscribers();
    }

    subscribe(callback: (() => void)): number {
        this._callbacks.push({callback, subscription: this._nextSubscriptionId++});
        return this._nextSubscriptionId - 1;
    }

    unsubscribe(subscriptionId?: number): void {
        if (subscriptionId === undefined)
            return;

        const subscriptionIndex = this._callbacks
            .map((element, index) => element.subscription === subscriptionId ? {
                found: true,
                index: index
            } : {found: false, index: 0})
            .filter(element => element.found);

        if (subscriptionIndex.length !== 1)
            throw new Error(`Found an invalid number of subscriptions ${subscriptionIndex.length}`);

        this._callbacks.splice(subscriptionIndex[0].index, 1);
    }

    notifySubscribers(): void {
        for (let i = 0; i < this._callbacks.length; i++) {
            const callback = this._callbacks[i].callback;
            callback();
        }
    }

    async logout(): Promise<boolean> {
        if (!this.isAuthenticated())
            return true;

        try {
            const request = new LogoutRequest().setSessionId(larpRestService.sessionId);
            await authRestService.logout(request);
        }
        catch (e) {
            console.log("Logout failed");
            console.log(e);
        }
        finally {
            this.updateState(undefined);
        }
        return true;
    }

    async validateSession(): Promise<boolean> {
        if (!this.isAuthenticated())
            return false;

        const request = new ValidateSessionRequest().setSessionId(larpRestService.sessionId);
        const result = await authRestService.validateSession(request);

        console.log("Validation result: " + result.getStatusCode());
        if (result.getStatusCode() === ValidationResponseCode.SUCCESS)
            return true;

        switch (result.getStatusCode()) {
            case ValidationResponseCode.INVALID:
                console.log("Session is invalid!");
                break;
            case ValidationResponseCode.EXPIRED:
                console.log("Session expired!");
                break;
            default:
                console.log("Unexpected session state!");
                break;
        }

        this.updateState(undefined);
        return false;
    }

    async login(email: string): Promise<LoginStatus> {
        this._email = email;
        const result = await authRestService.initiateLogin(new InitiateLoginRequest().setEmail(email));
        switch (result.getStatusCode()) {
            case ValidationResponseCode.SUCCESS:
                return LoginStatus.Success;
            default:
                return LoginStatus.Failed;
        }
    }

    async confirm(email: string, code: string): Promise<ConfirmStatus> {
        const req = new ConfirmLoginRequest().setEmail(email).setCode(code);
        const result = await authRestService.confirmLogin(req);
        switch (result.getStatusCode()) {
            case ValidationResponseCode.SUCCESS:
                // TODO cache profile
                this.updateState(result.getSessionId());
                return ConfirmStatus.Success;
            case ValidationResponseCode.EXPIRED:
                return ConfirmStatus.Expired;
            default:
                return ConfirmStatus.Invalid
        }
    }

    static get instance(): SessionService {
        return sessionService
    }

    isAdmin() {
        return this._account.isSuperAdmin;
    }

    async getAccount(): Promise<Account.AsObject> {
        const result = await userRestService.getAccount();
        return this.returnAccount(result);
    }

    async getGameState(): Promise<GameState.AsObject> {
        const cacheName = "mw5e";

        if (!this._mw5eGameState.isExpired())
            return this._mw5eGameState.get()!;

        // Initial load or check for updates
        try {
            const lastRevision = this._mw5eGameState.getRevision() ?? "";
            const response = await larpMw5eService.getGameState(lastRevision);
            if (response.hasGameState()) {
                console.log("Cached GameState updated for " + cacheName)
                const newState: GameState.AsObject = response.getGameState()?.toObject()!;
                this._mw5eGameState.set(newState);
            } else {
                // Cache is current
                this._mw5eGameState.current();
            }
            return this._mw5eGameState.get()!;
        } catch (e) {
            const result = this._mw5eGameState.get();
            if (result === undefined)
                throw e;
            console.log("Failed to update GameState for " + cacheName + ", using cache");
            return result;
        }
    }

    async setProfile(name?: string, phone?: string, location?: string, notes?: string): Promise<Account.AsObject> {
        const account = new Account().toObject();

        const request = new UpdateProfileRequest();
        if (name && name !== account.name)
            request.setName(name);
        if (phone && phone !== account.phone)
            request.setPhone(phone);
        if (location && location !== account.location)
            request.setLocation(location);
        if (notes && notes !== account.notes)
            request.setNotes(notes);

        const result = await userRestService.updateProfile(request);
        return this.returnAccount(result);
    }

    returnAccount(response: AccountResponse): Account.AsObject {
        const account = (response.getAccount() ?? new Account()).toObject();
        this._account = account;
        return account;
    }

    async addEmail(email: string): Promise<Account.AsObject> {
        const result = await userRestService.addEmail(email);
        return this.returnAccount(result);
    }

    async removeEmail(email: string): Promise<Account.AsObject> {
        const result = await userRestService.removeEmail(email);
        return this.returnAccount(result);
    }

    async preferredEmail(email: string): Promise<Account.AsObject> {
        const result = await userRestService.preferEmail(email);
        return this.returnAccount(result);
    }

    async rsvp(id: string, rsvp: EventRsvp) {
        const request = new EventRsvpRequest()
            .setEventId(id)
            .setRsvp(rsvp);
        await userRestService.rsvpEvent(request);
    }

    async getEvents(includePast: boolean, includeFuture: boolean, includeAttendance: boolean): Promise<Event.AsObject[]> {
        const request = new EventListRequest()
            .setIncludePast(includePast)
            .setIncludeFuture(includeFuture)
            .setIncludeAttendance(includeAttendance);
        const response = await userRestService.getEvents(request);
        return response.toObject().eventList;
    }

    async getEvent(id: string): Promise<Event.AsObject> {
        const response = await userRestService.getEvent(new EventRequest().setEventId(id))
        return response.toObject();
    }

    async getCharacters(): Promise<Character.AsObject[]> {
        return [] as Character.AsObject[];
    }
}

const sessionService: SessionService = new SessionService();

export default sessionService;
