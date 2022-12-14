/**
 * @fileoverview gRPC-Web generated client stub for larp.mw5e
 * @enhanceable
 * @public
 */

// Code generated by protoc-gen-grpc-web. DO NOT EDIT.
// versions:
// 	protoc-gen-grpc-web v1.4.1
// 	protoc              v3.21.9
// source: larp/mw5e/services.proto


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as larp_mw5e_character_pb from '../../larp/mw5e/character_pb';
import * as larp_mw5e_services_pb from '../../larp/mw5e/services_pb';


export class Mw5eClient {
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

  methodDescriptorGetGameState = new grpcWeb.MethodDescriptor(
    '/larp.mw5e.Mw5e/GetGameState',
    grpcWeb.MethodType.UNARY,
    larp_mw5e_services_pb.UpdateCacheRequest,
    larp_mw5e_services_pb.GameStateResponse,
    (request: larp_mw5e_services_pb.UpdateCacheRequest) => {
      return request.serializeBinary();
    },
    larp_mw5e_services_pb.GameStateResponse.deserializeBinary
  );

  getGameState(
    request: larp_mw5e_services_pb.UpdateCacheRequest,
    metadata: grpcWeb.Metadata | null): Promise<larp_mw5e_services_pb.GameStateResponse>;

  getGameState(
    request: larp_mw5e_services_pb.UpdateCacheRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: larp_mw5e_services_pb.GameStateResponse) => void): grpcWeb.ClientReadableStream<larp_mw5e_services_pb.GameStateResponse>;

  getGameState(
    request: larp_mw5e_services_pb.UpdateCacheRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: larp_mw5e_services_pb.GameStateResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/larp.mw5e.Mw5e/GetGameState',
        request,
        metadata || {},
        this.methodDescriptorGetGameState,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/larp.mw5e.Mw5e/GetGameState',
    request,
    metadata || {},
    this.methodDescriptorGetGameState);
  }

  methodDescriptorGetCharacter = new grpcWeb.MethodDescriptor(
    '/larp.mw5e.Mw5e/GetCharacter',
    grpcWeb.MethodType.UNARY,
    larp_mw5e_services_pb.FindById,
    larp_mw5e_character_pb.Character,
    (request: larp_mw5e_services_pb.FindById) => {
      return request.serializeBinary();
    },
    larp_mw5e_character_pb.Character.deserializeBinary
  );

  getCharacter(
    request: larp_mw5e_services_pb.FindById,
    metadata: grpcWeb.Metadata | null): Promise<larp_mw5e_character_pb.Character>;

  getCharacter(
    request: larp_mw5e_services_pb.FindById,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: larp_mw5e_character_pb.Character) => void): grpcWeb.ClientReadableStream<larp_mw5e_character_pb.Character>;

  getCharacter(
    request: larp_mw5e_services_pb.FindById,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: larp_mw5e_character_pb.Character) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/larp.mw5e.Mw5e/GetCharacter',
        request,
        metadata || {},
        this.methodDescriptorGetCharacter,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/larp.mw5e.Mw5e/GetCharacter',
    request,
    metadata || {},
    this.methodDescriptorGetCharacter);
  }

}

