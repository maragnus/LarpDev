import {
    ConfirmLoginRequest,
    ConfirmLoginResponse,
    InitiateLoginRequest,
    InitiateLoginResponse,
    LogoutRequest,
    LogoutResponse,
    ValidateSessionRequest,
    ValidateSessionResponse
} from "./Protos/larp/authorization";
import {Empty, StringRequest} from "./Protos/larp/common";
import {
    AccountResponse,
    EventListRequest,
    EventListResponse,
    EventRequest,
    EventRsvpRequest,
    UpdateProfileRequest,
} from "./Protos/larp/services";
import {Event} from "./Protos/larp/events";
import {GameStateResponse, UpdateCacheRequest} from "./Protos/larp/mw5e/services";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:5001/msg/'
    : (window.location.origin + '/msg/');

class LarpRestService {
    public sessionId: string = 'bypass';

    async get<TResponse = any>(url: string, request: any, deserialize: (data: Uint8Array) => TResponse): Promise<TResponse> {
        const response = await fetch(host + url, {
            method: 'POST',
            mode: 'cors',
            cache: 'no-cache',
            headers: {
                'Content-Type': 'application/x-protobuf',
                'x-session-id': this.sessionId
            },
            redirect: 'error',
            referrerPolicy: 'no-referrer',
            body: request.serializeBinary()
        });
        const data = new Uint8Array(await response.arrayBuffer());
        return deserialize(data);
    }

    async call(url: string, request: any) {
        await fetch(host + url, {
            method: 'POST',
            mode: 'cors',
            cache: 'no-cache',
            headers: {
                'Content-Type': 'application/x-protobuf',
                'x-session-id': this.sessionId
            },
            redirect: 'error',
            referrerPolicy: 'no-referrer',
            body: request.serializeBinary()
        });
    }
}

export const larpRestService = new LarpRestService();

class AuthRestService {
    async initiateLogin(request: InitiateLoginRequest): Promise<InitiateLoginResponse> {
        return await larpRestService.get("Auth/InitiateLogin", request, InitiateLoginResponse.decode)
    }

    async confirmLogin(request: ConfirmLoginRequest): Promise<ConfirmLoginResponse> {
        return await larpRestService.get("Auth/ConfirmLogin", request, ConfirmLoginResponse.decode)
    }

    async validateSession(request: ValidateSessionRequest): Promise<ValidateSessionResponse> {
        return await larpRestService.get("Auth/ValidateSession", request, ValidateSessionResponse.decode)
    }

    async logout(request: LogoutRequest): Promise<LogoutResponse> {
        return await larpRestService.get("Auth/Logout", request, LogoutResponse.decode)
    }

}

class UserRestService {
    async addEmail(request: string): Promise<AccountResponse> {
        return await larpRestService.get("User/AddEmail", {value: request} as StringRequest, AccountResponse.decode)
    }

    async removeEmail(request: string): Promise<AccountResponse> {
        return await larpRestService.get("User/PreferEmail", {value: request} as StringRequest, AccountResponse.decode)
    }

    async preferEmail(request: string): Promise<AccountResponse> {
        return await larpRestService.get("User/PreferEmail", {value: request} as StringRequest, AccountResponse.decode)
    }

    async updateProfile(request: UpdateProfileRequest): Promise<AccountResponse> {
        return await larpRestService.get("User/UpdateProfile", request, AccountResponse.decode)
    }

    async getAccount(): Promise<AccountResponse> {
        return await larpRestService.get("User/GetAccount", {} as Empty, AccountResponse.decode)
    }

    async getEvents(request: EventListRequest): Promise<EventListResponse> {
        return await larpRestService.get("User/GetEvents", request, EventListResponse.decode)
    }

    async rsvpEvent(request: EventRsvpRequest): Promise<Event> {
        return await larpRestService.get("User/RsvpEvent", request, Event.decode)
    }

    async getEvent(request: EventRequest): Promise<Event> {
        return await larpRestService.get("User/GetEvent", request, Event.decode)
    }
}

class LarpMw5eService {
    async getGameState(lastRevision: string): Promise<GameStateResponse> {
        const request = { lastUpdated: lastRevision } as UpdateCacheRequest;
        return await larpRestService.get("Mw5e/GetGameState", request, GameStateResponse.decode);
    }
}

export const authRestService = new AuthRestService();
export const userRestService = new UserRestService();
export const larpMw5eService = new LarpMw5eService();