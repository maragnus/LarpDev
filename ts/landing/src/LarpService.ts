import {LarpAuthenticationClient} from "./Protos/larp/AuthorizationServiceClientPb";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:44330'
    : 'https://mystwoodlanding.azurewebsites.net';

export const larpAuthClient = new LarpAuthenticationClient(host);