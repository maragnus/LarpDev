import {authRestService, larpMw5eService, larpRestService, userRestService} from "./LarpService";
import {
    LogoutRequest,
    ValidateSessionRequest,
    ValidationResponseCode
} from "./Protos/larp/authorization";
import {Account} from "./Protos/larp/accounts";
import {GameState} from "./Protos/larp/mw5e/fifthedition";
import {CachedGameState} from "./CachedGameState";
import {
    AccountResponse,
    EventListRequest,
    EventRequest,
    EventRsvpRequest,
    UpdateProfileRequest
} from "./Protos/larp/services";
import {Event, EventRsvp} from "./Protos/larp/events";
import {Character} from "./Protos/larp/mw5e/character";

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
    private _mw5eGameState = new CachedGameState<GameState>("mw5e");
    private _account: Account = {} as Account;

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
            const request = {sessionId: larpRestService.sessionId} as LogoutRequest;
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

        const request = { sessionId: larpRestService.sessionId } as ValidateSessionRequest;
        const result = await authRestService.validateSession(request);

        console.log("Validation result: " + result.statusCode);
        if (result.statusCode === ValidationResponseCode.SUCCESS)
            return true;

        switch (result.statusCode) {
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
        const result = await authRestService.initiateLogin({ email: email });
        switch (result.statusCode) {
            case ValidationResponseCode.SUCCESS:
                return LoginStatus.Success;
            default:
                return LoginStatus.Failed;
        }
    }

    async confirm(email: string, code: string): Promise<ConfirmStatus> {
        const result = await authRestService.confirmLogin({email: email, code: code});
        switch (result.statusCode) {
            case ValidationResponseCode.SUCCESS:
                // TODO cache profile
                this.updateState(result.sessionId);
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

    async getAccount(): Promise<Account> {
        const result = await userRestService.getAccount();
        return this.returnAccount(result);
    }

    async getGameState(): Promise<GameState> {
        const cacheName = "mw5e";

        if (!this._mw5eGameState.isExpired())
            return this._mw5eGameState.get()!;

        // Initial load or check for updates
        try {
            const lastRevision = this._mw5eGameState.getRevision() ?? "";
            const response = await larpMw5eService.getGameState(lastRevision);
            if (response.gameState !== undefined) {
                console.log("Cached GameState updated for " + cacheName)
                const newState: GameState = response.gameState;
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

    async setProfile(name?: string, phone?: string, location?: string, notes?: string): Promise<Account> {
        const account = {} as Account;

        const request = {} as UpdateProfileRequest;
        if (name && name !== account.name)
            request.name = name;
        if (phone && phone !== account.phone)
            request.phone = phone;
        if (location && location !== account.location)
            request.location = location;
        if (notes && notes !== account.notes)
            request.notes = notes;

        const result = await userRestService.updateProfile(request);
        return this.returnAccount(result);
    }

    returnAccount(response: AccountResponse): Account {
        const account = (response.account ?? {} as Account);
        this._account = account;
        return account;
    }

    async addEmail(email: string): Promise<Account> {
        const result = await userRestService.addEmail(email);
        return this.returnAccount(result);
    }

    async removeEmail(email: string): Promise<Account> {
        const result = await userRestService.removeEmail(email);
        return this.returnAccount(result);
    }

    async preferredEmail(email: string): Promise<Account> {
        const result = await userRestService.preferEmail(email);
        return this.returnAccount(result);
    }

    async rsvp(id: string, rsvp: EventRsvp) {
        const request = {eventId: id, rsvp: rsvp } as EventRsvpRequest;
        await userRestService.rsvpEvent(request);
    }

    async getEvents(includePast: boolean, includeFuture: boolean, includeAttendance: boolean): Promise<Event[]> {
        const request = { includePast: includePast, includeFuture:includeFuture, includeAttendance:includeAttendance } as EventListRequest;
        const response = await userRestService.getEvents(request);
        return response.event;
    }

    async getEvent(id: string): Promise<Event> {
        const response = await userRestService.getEvent({eventId: id} as EventRequest)
        return response;
    }

    async getCharacters(): Promise<Character[]> {
        return [] as Character[];
    }
}

const sessionService: SessionService = new SessionService();

export default sessionService;
