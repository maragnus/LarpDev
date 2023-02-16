/* eslint-disable */
import * as _m0 from "protobufjs/minimal";
import { Account } from "./accounts";

export const protobufPackage = "larp.authorization";

export const ValidationResponseCode = { SUCCESS: 0, EXPIRED: 1, INVALID: 2, UNRECOGNIZED: -1 } as const;

export type ValidationResponseCode = typeof ValidationResponseCode[keyof typeof ValidationResponseCode];

export function validationResponseCodeFromJSON(object: any): ValidationResponseCode {
  switch (object) {
    case 0:
    case "SUCCESS":
      return ValidationResponseCode.SUCCESS;
    case 1:
    case "EXPIRED":
      return ValidationResponseCode.EXPIRED;
    case 2:
    case "INVALID":
      return ValidationResponseCode.INVALID;
    case -1:
    case "UNRECOGNIZED":
    default:
      return ValidationResponseCode.UNRECOGNIZED;
  }
}

export function validationResponseCodeToJSON(object: ValidationResponseCode): string {
  switch (object) {
    case ValidationResponseCode.SUCCESS:
      return "SUCCESS";
    case ValidationResponseCode.EXPIRED:
      return "EXPIRED";
    case ValidationResponseCode.INVALID:
      return "INVALID";
    case ValidationResponseCode.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface InitiateLoginRequest {
  email: string;
}

export interface InitiateLoginResponse {
  statusCode: ValidationResponseCode;
  message: string;
}

export interface ConfirmLoginRequest {
  email: string;
  code: string;
}

export interface ConfirmLoginResponse {
  sessionId: string;
  statusCode: ValidationResponseCode;
  message: string;
  profile: Account | undefined;
}

export interface ValidateSessionRequest {
  sessionId: string;
}

export interface ValidateSessionResponse {
  statusCode: ValidationResponseCode;
  profile?: Account | undefined;
}

export interface LogoutRequest {
  sessionId: string;
}

export interface LogoutResponse {
}

function createBaseInitiateLoginRequest(): InitiateLoginRequest {
  return { email: "" };
}

export const InitiateLoginRequest = {
  encode(message: InitiateLoginRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.email !== "") {
      writer.uint32(10).string(message.email);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): InitiateLoginRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseInitiateLoginRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.email = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<InitiateLoginRequest, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<InitiateLoginRequest | InitiateLoginRequest[]>
      | Iterable<InitiateLoginRequest | InitiateLoginRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [InitiateLoginRequest.encode(p).finish()];
        }
      } else {
        yield* [InitiateLoginRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, InitiateLoginRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<InitiateLoginRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [InitiateLoginRequest.decode(p)];
        }
      } else {
        yield* [InitiateLoginRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): InitiateLoginRequest {
    return { email: isSet(object.email) ? String(object.email) : "" };
  },

  toJSON(message: InitiateLoginRequest): unknown {
    const obj: any = {};
    message.email !== undefined && (obj.email = message.email);
    return obj;
  },

  create<I extends Exact<DeepPartial<InitiateLoginRequest>, I>>(base?: I): InitiateLoginRequest {
    return InitiateLoginRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<InitiateLoginRequest>, I>>(object: I): InitiateLoginRequest {
    const message = createBaseInitiateLoginRequest();
    message.email = object.email ?? "";
    return message;
  },
};

function createBaseInitiateLoginResponse(): InitiateLoginResponse {
  return { statusCode: 0, message: "" };
}

export const InitiateLoginResponse = {
  encode(message: InitiateLoginResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.statusCode !== 0) {
      writer.uint32(8).int32(message.statusCode);
    }
    if (message.message !== "") {
      writer.uint32(18).string(message.message);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): InitiateLoginResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseInitiateLoginResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.statusCode = reader.int32() as any;
          break;
        case 2:
          message.message = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<InitiateLoginResponse, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<InitiateLoginResponse | InitiateLoginResponse[]>
      | Iterable<InitiateLoginResponse | InitiateLoginResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [InitiateLoginResponse.encode(p).finish()];
        }
      } else {
        yield* [InitiateLoginResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, InitiateLoginResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<InitiateLoginResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [InitiateLoginResponse.decode(p)];
        }
      } else {
        yield* [InitiateLoginResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): InitiateLoginResponse {
    return {
      statusCode: isSet(object.statusCode) ? validationResponseCodeFromJSON(object.statusCode) : 0,
      message: isSet(object.message) ? String(object.message) : "",
    };
  },

  toJSON(message: InitiateLoginResponse): unknown {
    const obj: any = {};
    message.statusCode !== undefined && (obj.statusCode = validationResponseCodeToJSON(message.statusCode));
    message.message !== undefined && (obj.message = message.message);
    return obj;
  },

  create<I extends Exact<DeepPartial<InitiateLoginResponse>, I>>(base?: I): InitiateLoginResponse {
    return InitiateLoginResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<InitiateLoginResponse>, I>>(object: I): InitiateLoginResponse {
    const message = createBaseInitiateLoginResponse();
    message.statusCode = object.statusCode ?? 0;
    message.message = object.message ?? "";
    return message;
  },
};

function createBaseConfirmLoginRequest(): ConfirmLoginRequest {
  return { email: "", code: "" };
}

export const ConfirmLoginRequest = {
  encode(message: ConfirmLoginRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.email !== "") {
      writer.uint32(10).string(message.email);
    }
    if (message.code !== "") {
      writer.uint32(18).string(message.code);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): ConfirmLoginRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseConfirmLoginRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.email = reader.string();
          break;
        case 2:
          message.code = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<ConfirmLoginRequest, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<ConfirmLoginRequest | ConfirmLoginRequest[]>
      | Iterable<ConfirmLoginRequest | ConfirmLoginRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ConfirmLoginRequest.encode(p).finish()];
        }
      } else {
        yield* [ConfirmLoginRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, ConfirmLoginRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<ConfirmLoginRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ConfirmLoginRequest.decode(p)];
        }
      } else {
        yield* [ConfirmLoginRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): ConfirmLoginRequest {
    return {
      email: isSet(object.email) ? String(object.email) : "",
      code: isSet(object.code) ? String(object.code) : "",
    };
  },

  toJSON(message: ConfirmLoginRequest): unknown {
    const obj: any = {};
    message.email !== undefined && (obj.email = message.email);
    message.code !== undefined && (obj.code = message.code);
    return obj;
  },

  create<I extends Exact<DeepPartial<ConfirmLoginRequest>, I>>(base?: I): ConfirmLoginRequest {
    return ConfirmLoginRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<ConfirmLoginRequest>, I>>(object: I): ConfirmLoginRequest {
    const message = createBaseConfirmLoginRequest();
    message.email = object.email ?? "";
    message.code = object.code ?? "";
    return message;
  },
};

function createBaseConfirmLoginResponse(): ConfirmLoginResponse {
  return { sessionId: "", statusCode: 0, message: "", profile: undefined };
}

export const ConfirmLoginResponse = {
  encode(message: ConfirmLoginResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.sessionId !== "") {
      writer.uint32(10).string(message.sessionId);
    }
    if (message.statusCode !== 0) {
      writer.uint32(16).int32(message.statusCode);
    }
    if (message.message !== "") {
      writer.uint32(26).string(message.message);
    }
    if (message.profile !== undefined) {
      Account.encode(message.profile, writer.uint32(34).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): ConfirmLoginResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseConfirmLoginResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.sessionId = reader.string();
          break;
        case 2:
          message.statusCode = reader.int32() as any;
          break;
        case 3:
          message.message = reader.string();
          break;
        case 4:
          message.profile = Account.decode(reader, reader.uint32());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<ConfirmLoginResponse, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<ConfirmLoginResponse | ConfirmLoginResponse[]>
      | Iterable<ConfirmLoginResponse | ConfirmLoginResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ConfirmLoginResponse.encode(p).finish()];
        }
      } else {
        yield* [ConfirmLoginResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, ConfirmLoginResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<ConfirmLoginResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ConfirmLoginResponse.decode(p)];
        }
      } else {
        yield* [ConfirmLoginResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): ConfirmLoginResponse {
    return {
      sessionId: isSet(object.sessionId) ? String(object.sessionId) : "",
      statusCode: isSet(object.statusCode) ? validationResponseCodeFromJSON(object.statusCode) : 0,
      message: isSet(object.message) ? String(object.message) : "",
      profile: isSet(object.profile) ? Account.fromJSON(object.profile) : undefined,
    };
  },

  toJSON(message: ConfirmLoginResponse): unknown {
    const obj: any = {};
    message.sessionId !== undefined && (obj.sessionId = message.sessionId);
    message.statusCode !== undefined && (obj.statusCode = validationResponseCodeToJSON(message.statusCode));
    message.message !== undefined && (obj.message = message.message);
    message.profile !== undefined && (obj.profile = message.profile ? Account.toJSON(message.profile) : undefined);
    return obj;
  },

  create<I extends Exact<DeepPartial<ConfirmLoginResponse>, I>>(base?: I): ConfirmLoginResponse {
    return ConfirmLoginResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<ConfirmLoginResponse>, I>>(object: I): ConfirmLoginResponse {
    const message = createBaseConfirmLoginResponse();
    message.sessionId = object.sessionId ?? "";
    message.statusCode = object.statusCode ?? 0;
    message.message = object.message ?? "";
    message.profile = (object.profile !== undefined && object.profile !== null)
      ? Account.fromPartial(object.profile)
      : undefined;
    return message;
  },
};

function createBaseValidateSessionRequest(): ValidateSessionRequest {
  return { sessionId: "" };
}

export const ValidateSessionRequest = {
  encode(message: ValidateSessionRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.sessionId !== "") {
      writer.uint32(10).string(message.sessionId);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): ValidateSessionRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseValidateSessionRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.sessionId = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<ValidateSessionRequest, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<ValidateSessionRequest | ValidateSessionRequest[]>
      | Iterable<ValidateSessionRequest | ValidateSessionRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ValidateSessionRequest.encode(p).finish()];
        }
      } else {
        yield* [ValidateSessionRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, ValidateSessionRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<ValidateSessionRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ValidateSessionRequest.decode(p)];
        }
      } else {
        yield* [ValidateSessionRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): ValidateSessionRequest {
    return { sessionId: isSet(object.sessionId) ? String(object.sessionId) : "" };
  },

  toJSON(message: ValidateSessionRequest): unknown {
    const obj: any = {};
    message.sessionId !== undefined && (obj.sessionId = message.sessionId);
    return obj;
  },

  create<I extends Exact<DeepPartial<ValidateSessionRequest>, I>>(base?: I): ValidateSessionRequest {
    return ValidateSessionRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<ValidateSessionRequest>, I>>(object: I): ValidateSessionRequest {
    const message = createBaseValidateSessionRequest();
    message.sessionId = object.sessionId ?? "";
    return message;
  },
};

function createBaseValidateSessionResponse(): ValidateSessionResponse {
  return { statusCode: 0, profile: undefined };
}

export const ValidateSessionResponse = {
  encode(message: ValidateSessionResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.statusCode !== 0) {
      writer.uint32(8).int32(message.statusCode);
    }
    if (message.profile !== undefined) {
      Account.encode(message.profile, writer.uint32(18).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): ValidateSessionResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseValidateSessionResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.statusCode = reader.int32() as any;
          break;
        case 2:
          message.profile = Account.decode(reader, reader.uint32());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<ValidateSessionResponse, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<ValidateSessionResponse | ValidateSessionResponse[]>
      | Iterable<ValidateSessionResponse | ValidateSessionResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ValidateSessionResponse.encode(p).finish()];
        }
      } else {
        yield* [ValidateSessionResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, ValidateSessionResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<ValidateSessionResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [ValidateSessionResponse.decode(p)];
        }
      } else {
        yield* [ValidateSessionResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): ValidateSessionResponse {
    return {
      statusCode: isSet(object.statusCode) ? validationResponseCodeFromJSON(object.statusCode) : 0,
      profile: isSet(object.profile) ? Account.fromJSON(object.profile) : undefined,
    };
  },

  toJSON(message: ValidateSessionResponse): unknown {
    const obj: any = {};
    message.statusCode !== undefined && (obj.statusCode = validationResponseCodeToJSON(message.statusCode));
    message.profile !== undefined && (obj.profile = message.profile ? Account.toJSON(message.profile) : undefined);
    return obj;
  },

  create<I extends Exact<DeepPartial<ValidateSessionResponse>, I>>(base?: I): ValidateSessionResponse {
    return ValidateSessionResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<ValidateSessionResponse>, I>>(object: I): ValidateSessionResponse {
    const message = createBaseValidateSessionResponse();
    message.statusCode = object.statusCode ?? 0;
    message.profile = (object.profile !== undefined && object.profile !== null)
      ? Account.fromPartial(object.profile)
      : undefined;
    return message;
  },
};

function createBaseLogoutRequest(): LogoutRequest {
  return { sessionId: "" };
}

export const LogoutRequest = {
  encode(message: LogoutRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.sessionId !== "") {
      writer.uint32(10).string(message.sessionId);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): LogoutRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseLogoutRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.sessionId = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<LogoutRequest, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<LogoutRequest | LogoutRequest[]> | Iterable<LogoutRequest | LogoutRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [LogoutRequest.encode(p).finish()];
        }
      } else {
        yield* [LogoutRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, LogoutRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<LogoutRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [LogoutRequest.decode(p)];
        }
      } else {
        yield* [LogoutRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): LogoutRequest {
    return { sessionId: isSet(object.sessionId) ? String(object.sessionId) : "" };
  },

  toJSON(message: LogoutRequest): unknown {
    const obj: any = {};
    message.sessionId !== undefined && (obj.sessionId = message.sessionId);
    return obj;
  },

  create<I extends Exact<DeepPartial<LogoutRequest>, I>>(base?: I): LogoutRequest {
    return LogoutRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<LogoutRequest>, I>>(object: I): LogoutRequest {
    const message = createBaseLogoutRequest();
    message.sessionId = object.sessionId ?? "";
    return message;
  },
};

function createBaseLogoutResponse(): LogoutResponse {
  return {};
}

export const LogoutResponse = {
  encode(_: LogoutResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): LogoutResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseLogoutResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<LogoutResponse, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<LogoutResponse | LogoutResponse[]> | Iterable<LogoutResponse | LogoutResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [LogoutResponse.encode(p).finish()];
        }
      } else {
        yield* [LogoutResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, LogoutResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<LogoutResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [LogoutResponse.decode(p)];
        }
      } else {
        yield* [LogoutResponse.decode(pkt)];
      }
    }
  },

  fromJSON(_: any): LogoutResponse {
    return {};
  },

  toJSON(_: LogoutResponse): unknown {
    const obj: any = {};
    return obj;
  },

  create<I extends Exact<DeepPartial<LogoutResponse>, I>>(base?: I): LogoutResponse {
    return LogoutResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<LogoutResponse>, I>>(_: I): LogoutResponse {
    const message = createBaseLogoutResponse();
    return message;
  },
};

export interface LarpAuthentication {
  initiateLogin(request: InitiateLoginRequest): Promise<InitiateLoginResponse>;
  confirmLogin(request: ConfirmLoginRequest): Promise<ConfirmLoginResponse>;
  validateSession(request: ValidateSessionRequest): Promise<ValidateSessionResponse>;
  logout(request: LogoutRequest): Promise<LogoutResponse>;
}

export class LarpAuthenticationClientImpl implements LarpAuthentication {
  private readonly rpc: Rpc;
  private readonly service: string;
  constructor(rpc: Rpc, opts?: { service?: string }) {
    this.service = opts?.service || "larp.authorization.LarpAuthentication";
    this.rpc = rpc;
    this.initiateLogin = this.initiateLogin.bind(this);
    this.confirmLogin = this.confirmLogin.bind(this);
    this.validateSession = this.validateSession.bind(this);
    this.logout = this.logout.bind(this);
  }
  initiateLogin(request: InitiateLoginRequest): Promise<InitiateLoginResponse> {
    const data = InitiateLoginRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "InitiateLogin", data);
    return promise.then((data) => InitiateLoginResponse.decode(new _m0.Reader(data)));
  }

  confirmLogin(request: ConfirmLoginRequest): Promise<ConfirmLoginResponse> {
    const data = ConfirmLoginRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "ConfirmLogin", data);
    return promise.then((data) => ConfirmLoginResponse.decode(new _m0.Reader(data)));
  }

  validateSession(request: ValidateSessionRequest): Promise<ValidateSessionResponse> {
    const data = ValidateSessionRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "ValidateSession", data);
    return promise.then((data) => ValidateSessionResponse.decode(new _m0.Reader(data)));
  }

  logout(request: LogoutRequest): Promise<LogoutResponse> {
    const data = LogoutRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "Logout", data);
    return promise.then((data) => LogoutResponse.decode(new _m0.Reader(data)));
  }
}

interface Rpc {
  request(service: string, method: string, data: Uint8Array): Promise<Uint8Array>;
}

type Builtin = Date | Function | Uint8Array | string | number | boolean | undefined;

export type DeepPartial<T> = T extends Builtin ? T
  : T extends Array<infer U> ? Array<DeepPartial<U>> : T extends ReadonlyArray<infer U> ? ReadonlyArray<DeepPartial<U>>
  : T extends {} ? { [K in keyof T]?: DeepPartial<T[K]> }
  : Partial<T>;

type KeysOfUnion<T> = T extends T ? keyof T : never;
export type Exact<P, I extends P> = P extends Builtin ? P
  : P & { [K in keyof P]: Exact<P[K], I[K]> } & { [K in Exclude<keyof I, KeysOfUnion<P>>]: never };

function isSet(value: any): boolean {
  return value !== null && value !== undefined;
}
