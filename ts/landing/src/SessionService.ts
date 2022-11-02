import {larpAuthClient} from "./LarpService";
import {ConfirmLoginRequest, InitiateLoginRequest, ValidationResponseCode} from "./Protos/larp/authorization_pb";

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
    private _sessionId?: string;

    private static readonly SessionIdKey = "MWL_SESSION_ID";
    private static readonly AccountKey = "MWL_PROFILE";

    constructor() {
        const sessionId = localStorage.getItem(SessionService.SessionIdKey) ?? "";
        if (sessionId !== undefined)
            this._sessionId = sessionId;

        this.startInterval();
    }

    startInterval() {
        // Make sure that all sessions identify if we are signed out
        setInterval(function () {
            let sessionId: string | undefined = localStorage.getItem(SessionService.SessionIdKey) ?? "";
            if (sessionId === "") sessionId = undefined;

            if (sessionId !== sessionService._sessionId)
                sessionService.updateState(sessionId);
        }, 1000);
    }

    isAuthenticated(): boolean {
        return this._sessionId !== undefined;
    }

    updateState(sessionId?: string): void {
        this._sessionId = sessionId;
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
        // TODO -- Send logout rpc
        this.updateState(undefined);
        return true;
    }

    async login(email: string): Promise<LoginStatus> {
        this._email = email;
        const result = await larpAuthClient.initiateLogin(new InitiateLoginRequest().setEmail(email), null);
        switch (result.getStatusCode()) {
            case ValidationResponseCode.SUCCESS:
                return LoginStatus.Success;
            default:
                return LoginStatus.Failed;
        }
    }

    async confirm(email: string, code: string): Promise<ConfirmStatus> {
        const req = new ConfirmLoginRequest().setEmail(email).setCode(code);
        const result = await larpAuthClient.confirmLogin( req, null);
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
        return false;
    }
}

const sessionService: SessionService = new SessionService();

export default sessionService;