import {LarpAuthenticationClient} from "./Protos/larp/AuthorizationServiceClientPb";
import {
    LarpUserClient,
} from "./Protos/larp/ServicesServiceClientPb";
import {Mw5eClient} from "./Protos/larp/mw5e/ServicesServiceClientPb";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:5001'
    : 'https://mystwoodlanding.azurewebsites.net';

export const larpAuthClient = new LarpAuthenticationClient(host, null, null);
export const larpUserClient = new LarpUserClient(host, null, null);
export const larpMw5eClient = new Mw5eClient(host, null, null);

