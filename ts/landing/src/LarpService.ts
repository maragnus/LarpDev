import {LarpAuthenticationClient} from "./Protos/larp/AuthorizationServiceClientPb";
import {grpc} from "@improbable-eng/grpc-web";
import {
    LarpAdminClient,
    LarpUserClient,
} from "./Protos/larp/ServicesServiceClientPb";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:5001'
    : 'https://mystwoodlanding.azurewebsites.net';

export const larpAuthClient = new LarpAuthenticationClient(host, null, null);
export const larpUserClient = new LarpUserClient(host, null, null);
export const larpAdminClient = new LarpAdminClient(host, null, null);
