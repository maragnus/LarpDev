import {
    ConfirmLoginRequest,
    ConfirmLoginResponse,
    InitiateLoginRequest,
    InitiateLoginResponse,
    LogoutRequest,
    LogoutResponse,
    ValidateSessionRequest,
    ValidateSessionResponse
} from "./Protos/larp/authorization_pb";
import {Empty, StringRequest} from "./Protos/larp/common_pb";
import {
    AccountResponse,
    EventListRequest,
    EventListResponse,
    EventRequest,
    EventRsvpRequest,
    UpdateProfileRequest,
} from "./Protos/larp/services_pb";
import {Event} from "./Protos/larp/events_pb";
import {GameStateResponse, UpdateCacheRequest} from "./Protos/larp/mw5e/services_pb";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:5001/msg/'
    : 'http://larp.maragnus.com/msg/';

class LarpRestService {
    async get<TResponse = any>(url: string, request: any, deserialize: (data: Uint8Array) => TResponse): Promise<TResponse> {
        const response = await fetch(host + url, {
            method: 'POST',
            mode: 'cors',
            cache: 'no-cache',
            headers: {
                'Content-Type': 'application/x-protobuf'
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
                'Content-Type': 'application/x-protobuf'
            },
            redirect: 'error',
            referrerPolicy: 'no-referrer',
            body: request.serializeBinary()
        });
    }
}

const rest = new LarpRestService();

class AuthRestService {
    async initiateLogin(request: InitiateLoginRequest): Promise<InitiateLoginResponse> {
        return await rest.get("Auth/InitiateLogin", request, InitiateLoginResponse.deserializeBinary)
    }

    async confirmLogin(request: ConfirmLoginRequest): Promise<ConfirmLoginResponse> {
        return await rest.get("Auth/ConfirmLogin", request, ConfirmLoginResponse.deserializeBinary)
    }

    async validateSession(request: ValidateSessionRequest): Promise<ValidateSessionResponse> {
        return await rest.get("Auth/ValidateSession", request, ValidateSessionResponse.deserializeBinary)
    }

    async logout(request: LogoutRequest): Promise<LogoutResponse> {
        return await rest.get("Auth/Logout", request, LogoutResponse.deserializeBinary)
    }

}

class UserRestService {
    async addEmail(request: string): Promise<AccountResponse> {
        return await rest.get("User/AddEmail", new StringRequest().setValue(request), AccountResponse.deserializeBinary)
    }

    async removeEmail(request: string): Promise<AccountResponse> {
        return await rest.get("User/PreferEmail", new StringRequest().setValue(request), AccountResponse.deserializeBinary)
    }

    async preferEmail(request: string): Promise<AccountResponse> {
        return await rest.get("User/PreferEmail", new StringRequest().setValue(request), AccountResponse.deserializeBinary)
    }

    async updateProfile(request: UpdateProfileRequest): Promise<AccountResponse> {
        return await rest.get("User/UpdateProfile", request, AccountResponse.deserializeBinary)
    }

    async getAccount(): Promise<AccountResponse> {
        return await rest.get("User/GetAccount", new Empty(), AccountResponse.deserializeBinary)
    }

    async getEvents(request: EventListRequest): Promise<EventListResponse> {
        return await rest.get("User/GetEvents", request, EventListResponse.deserializeBinary)
    }

    async rsvpEvent(request: EventRsvpRequest): Promise<Event> {
        return await rest.get("User/RsvpEvent", request, Event.deserializeBinary)
    }

    async getEvent(request: EventRequest): Promise<Event> {
        return await rest.get("User/GetEvent", request, Event.deserializeBinary)
    }
}

class LarpMw5eService {
    async getGameState(lastRevision: string): Promise<GameStateResponse> {
        const request = new UpdateCacheRequest().setLastUpdated(lastRevision);
        return await rest.get("Mw5e/GetGameState", request, GameStateResponse.deserializeBinary);
    }
}

export const authRestService = new AuthRestService();
export const userRestService = new UserRestService();
export const larpMw5eService = new LarpMw5eService();