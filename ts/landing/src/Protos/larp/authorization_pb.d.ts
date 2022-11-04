import * as jspb from 'google-protobuf'

import * as larp_accounts_pb from '../larp/accounts_pb';


export class InitiateLoginRequest extends jspb.Message {
  getEmail(): string;
  setEmail(value: string): InitiateLoginRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): InitiateLoginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: InitiateLoginRequest): InitiateLoginRequest.AsObject;
  static serializeBinaryToWriter(message: InitiateLoginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): InitiateLoginRequest;
  static deserializeBinaryFromReader(message: InitiateLoginRequest, reader: jspb.BinaryReader): InitiateLoginRequest;
}

export namespace InitiateLoginRequest {
  export type AsObject = {
    email: string,
  }
}

export class InitiateLoginResponse extends jspb.Message {
  getStatusCode(): ValidationResponseCode;
  setStatusCode(value: ValidationResponseCode): InitiateLoginResponse;

  getMessage(): string;
  setMessage(value: string): InitiateLoginResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): InitiateLoginResponse.AsObject;
  static toObject(includeInstance: boolean, msg: InitiateLoginResponse): InitiateLoginResponse.AsObject;
  static serializeBinaryToWriter(message: InitiateLoginResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): InitiateLoginResponse;
  static deserializeBinaryFromReader(message: InitiateLoginResponse, reader: jspb.BinaryReader): InitiateLoginResponse;
}

export namespace InitiateLoginResponse {
  export type AsObject = {
    statusCode: ValidationResponseCode,
    message: string,
  }
}

export class ConfirmLoginRequest extends jspb.Message {
  getEmail(): string;
  setEmail(value: string): ConfirmLoginRequest;

  getCode(): string;
  setCode(value: string): ConfirmLoginRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ConfirmLoginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ConfirmLoginRequest): ConfirmLoginRequest.AsObject;
  static serializeBinaryToWriter(message: ConfirmLoginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ConfirmLoginRequest;
  static deserializeBinaryFromReader(message: ConfirmLoginRequest, reader: jspb.BinaryReader): ConfirmLoginRequest;
}

export namespace ConfirmLoginRequest {
  export type AsObject = {
    email: string,
    code: string,
  }
}

export class ConfirmLoginResponse extends jspb.Message {
  getSessionId(): string;
  setSessionId(value: string): ConfirmLoginResponse;

  getStatusCode(): ValidationResponseCode;
  setStatusCode(value: ValidationResponseCode): ConfirmLoginResponse;

  getMessage(): string;
  setMessage(value: string): ConfirmLoginResponse;

  getProfile(): larp_accounts_pb.Account | undefined;
  setProfile(value?: larp_accounts_pb.Account): ConfirmLoginResponse;
  hasProfile(): boolean;
  clearProfile(): ConfirmLoginResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ConfirmLoginResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ConfirmLoginResponse): ConfirmLoginResponse.AsObject;
  static serializeBinaryToWriter(message: ConfirmLoginResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ConfirmLoginResponse;
  static deserializeBinaryFromReader(message: ConfirmLoginResponse, reader: jspb.BinaryReader): ConfirmLoginResponse;
}

export namespace ConfirmLoginResponse {
  export type AsObject = {
    sessionId: string,
    statusCode: ValidationResponseCode,
    message: string,
    profile?: larp_accounts_pb.Account.AsObject,
  }
}

export class ValidateSessionRequest extends jspb.Message {
  getSessionId(): string;
  setSessionId(value: string): ValidateSessionRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ValidateSessionRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ValidateSessionRequest): ValidateSessionRequest.AsObject;
  static serializeBinaryToWriter(message: ValidateSessionRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ValidateSessionRequest;
  static deserializeBinaryFromReader(message: ValidateSessionRequest, reader: jspb.BinaryReader): ValidateSessionRequest;
}

export namespace ValidateSessionRequest {
  export type AsObject = {
    sessionId: string,
  }
}

export class ValidateSessionResponse extends jspb.Message {
  getStatusCode(): ValidationResponseCode;
  setStatusCode(value: ValidationResponseCode): ValidateSessionResponse;

  getProfile(): larp_accounts_pb.Account | undefined;
  setProfile(value?: larp_accounts_pb.Account): ValidateSessionResponse;
  hasProfile(): boolean;
  clearProfile(): ValidateSessionResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ValidateSessionResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ValidateSessionResponse): ValidateSessionResponse.AsObject;
  static serializeBinaryToWriter(message: ValidateSessionResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ValidateSessionResponse;
  static deserializeBinaryFromReader(message: ValidateSessionResponse, reader: jspb.BinaryReader): ValidateSessionResponse;
}

export namespace ValidateSessionResponse {
  export type AsObject = {
    statusCode: ValidationResponseCode,
    profile?: larp_accounts_pb.Account.AsObject,
  }

  export enum ProfileCase { 
    _PROFILE_NOT_SET = 0,
    PROFILE = 2,
  }
}

export class LogoutRequest extends jspb.Message {
  getSessionId(): string;
  setSessionId(value: string): LogoutRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LogoutRequest.AsObject;
  static toObject(includeInstance: boolean, msg: LogoutRequest): LogoutRequest.AsObject;
  static serializeBinaryToWriter(message: LogoutRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LogoutRequest;
  static deserializeBinaryFromReader(message: LogoutRequest, reader: jspb.BinaryReader): LogoutRequest;
}

export namespace LogoutRequest {
  export type AsObject = {
    sessionId: string,
  }
}

export class LogoutResponse extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LogoutResponse.AsObject;
  static toObject(includeInstance: boolean, msg: LogoutResponse): LogoutResponse.AsObject;
  static serializeBinaryToWriter(message: LogoutResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LogoutResponse;
  static deserializeBinaryFromReader(message: LogoutResponse, reader: jspb.BinaryReader): LogoutResponse;
}

export namespace LogoutResponse {
  export type AsObject = {
  }
}

export enum ValidationResponseCode { 
  SUCCESS = 0,
  EXPIRED = 1,
  INVALID = 2,
}
