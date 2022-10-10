/**
 * @fileoverview gRPC-Web generated client stub for larp.authorization
 * @enhanceable
 * @public
 */

// Code generated by protoc-gen-grpc-web. DO NOT EDIT.
// versions:
// 	protoc-gen-grpc-web v1.4.1
// 	protoc              v3.21.7
// source: authorization.proto


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as authorization_pb from './authorization_pb';


export class LarpAuthenticationClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'binary';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname.replace(/\/+$/, '');
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodDescriptorInitiateLogin = new grpcWeb.MethodDescriptor(
    '/larp.authorization.LarpAuthentication/InitiateLogin',
    grpcWeb.MethodType.UNARY,
    authorization_pb.InitiateLoginRequest,
    authorization_pb.InitiateLoginResponse,
    (request: authorization_pb.InitiateLoginRequest) => {
      return request.serializeBinary();
    },
    authorization_pb.InitiateLoginResponse.deserializeBinary
  );

  initiateLogin(
    request: authorization_pb.InitiateLoginRequest,
    metadata: grpcWeb.Metadata | null): Promise<authorization_pb.InitiateLoginResponse>;

  initiateLogin(
    request: authorization_pb.InitiateLoginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: authorization_pb.InitiateLoginResponse) => void): grpcWeb.ClientReadableStream<authorization_pb.InitiateLoginResponse>;

  initiateLogin(
    request: authorization_pb.InitiateLoginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: authorization_pb.InitiateLoginResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/larp.authorization.LarpAuthentication/InitiateLogin',
        request,
        metadata || {},
        this.methodDescriptorInitiateLogin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/larp.authorization.LarpAuthentication/InitiateLogin',
    request,
    metadata || {},
    this.methodDescriptorInitiateLogin);
  }

  methodDescriptorConfirmLogin = new grpcWeb.MethodDescriptor(
    '/larp.authorization.LarpAuthentication/ConfirmLogin',
    grpcWeb.MethodType.UNARY,
    authorization_pb.ConfirmLoginRequest,
    authorization_pb.ConfirmLoginResponse,
    (request: authorization_pb.ConfirmLoginRequest) => {
      return request.serializeBinary();
    },
    authorization_pb.ConfirmLoginResponse.deserializeBinary
  );

  confirmLogin(
    request: authorization_pb.ConfirmLoginRequest,
    metadata: grpcWeb.Metadata | null): Promise<authorization_pb.ConfirmLoginResponse>;

  confirmLogin(
    request: authorization_pb.ConfirmLoginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: authorization_pb.ConfirmLoginResponse) => void): grpcWeb.ClientReadableStream<authorization_pb.ConfirmLoginResponse>;

  confirmLogin(
    request: authorization_pb.ConfirmLoginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: authorization_pb.ConfirmLoginResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/larp.authorization.LarpAuthentication/ConfirmLogin',
        request,
        metadata || {},
        this.methodDescriptorConfirmLogin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/larp.authorization.LarpAuthentication/ConfirmLogin',
    request,
    metadata || {},
    this.methodDescriptorConfirmLogin);
  }

  methodDescriptorValidateSession = new grpcWeb.MethodDescriptor(
    '/larp.authorization.LarpAuthentication/ValidateSession',
    grpcWeb.MethodType.UNARY,
    authorization_pb.ValidateSessionRequest,
    authorization_pb.ValidateSessionResponse,
    (request: authorization_pb.ValidateSessionRequest) => {
      return request.serializeBinary();
    },
    authorization_pb.ValidateSessionResponse.deserializeBinary
  );

  validateSession(
    request: authorization_pb.ValidateSessionRequest,
    metadata: grpcWeb.Metadata | null): Promise<authorization_pb.ValidateSessionResponse>;

  validateSession(
    request: authorization_pb.ValidateSessionRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: authorization_pb.ValidateSessionResponse) => void): grpcWeb.ClientReadableStream<authorization_pb.ValidateSessionResponse>;

  validateSession(
    request: authorization_pb.ValidateSessionRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: authorization_pb.ValidateSessionResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/larp.authorization.LarpAuthentication/ValidateSession',
        request,
        metadata || {},
        this.methodDescriptorValidateSession,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/larp.authorization.LarpAuthentication/ValidateSession',
    request,
    metadata || {},
    this.methodDescriptorValidateSession);
  }

}

