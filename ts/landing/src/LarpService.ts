import {LarpAuthenticationClient} from "./Protos/larp/AuthorizationServiceClientPb";
import {grpc} from "@improbable-eng/grpc-web";
import Metadata = grpc.Metadata;
import {GrpcWebClientBaseOptions, StreamInterceptor, UnaryInterceptor} from "grpc-web";

// eslint-disable-next-line no-restricted-globals
const host = location.hostname === 'localhost'
    ? 'https://localhost:5001'
    : 'https://mystwoodlanding.azurewebsites.net';

export const larpAuthClient = new LarpAuthenticationClient(host, null, null);
