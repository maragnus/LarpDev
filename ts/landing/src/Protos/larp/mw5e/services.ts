/* eslint-disable */
import * as _m0 from "protobufjs/minimal";
import { Character } from "./character";
import { GameState } from "./fifthedition";

export const protobufPackage = "larp.mw5e";

export interface UpdateCacheRequest {
  lastUpdated: string;
}

export interface GameStateResponse {
  gameState?: GameState | undefined;
}

export interface FindById {
  id: string;
}

function createBaseUpdateCacheRequest(): UpdateCacheRequest {
  return { lastUpdated: "" };
}

export const UpdateCacheRequest = {
  encode(message: UpdateCacheRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.lastUpdated !== "") {
      writer.uint32(10).string(message.lastUpdated);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): UpdateCacheRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseUpdateCacheRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.lastUpdated = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<UpdateCacheRequest, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<UpdateCacheRequest | UpdateCacheRequest[]>
      | Iterable<UpdateCacheRequest | UpdateCacheRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [UpdateCacheRequest.encode(p).finish()];
        }
      } else {
        yield* [UpdateCacheRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, UpdateCacheRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<UpdateCacheRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [UpdateCacheRequest.decode(p)];
        }
      } else {
        yield* [UpdateCacheRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): UpdateCacheRequest {
    return { lastUpdated: isSet(object.lastUpdated) ? String(object.lastUpdated) : "" };
  },

  toJSON(message: UpdateCacheRequest): unknown {
    const obj: any = {};
    message.lastUpdated !== undefined && (obj.lastUpdated = message.lastUpdated);
    return obj;
  },

  create<I extends Exact<DeepPartial<UpdateCacheRequest>, I>>(base?: I): UpdateCacheRequest {
    return UpdateCacheRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<UpdateCacheRequest>, I>>(object: I): UpdateCacheRequest {
    const message = createBaseUpdateCacheRequest();
    message.lastUpdated = object.lastUpdated ?? "";
    return message;
  },
};

function createBaseGameStateResponse(): GameStateResponse {
  return { gameState: undefined };
}

export const GameStateResponse = {
  encode(message: GameStateResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.gameState !== undefined) {
      GameState.encode(message.gameState, writer.uint32(10).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): GameStateResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGameStateResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.gameState = GameState.decode(reader, reader.uint32());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<GameStateResponse, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<GameStateResponse | GameStateResponse[]> | Iterable<GameStateResponse | GameStateResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GameStateResponse.encode(p).finish()];
        }
      } else {
        yield* [GameStateResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, GameStateResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<GameStateResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GameStateResponse.decode(p)];
        }
      } else {
        yield* [GameStateResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): GameStateResponse {
    return { gameState: isSet(object.gameState) ? GameState.fromJSON(object.gameState) : undefined };
  },

  toJSON(message: GameStateResponse): unknown {
    const obj: any = {};
    message.gameState !== undefined &&
      (obj.gameState = message.gameState ? GameState.toJSON(message.gameState) : undefined);
    return obj;
  },

  create<I extends Exact<DeepPartial<GameStateResponse>, I>>(base?: I): GameStateResponse {
    return GameStateResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<GameStateResponse>, I>>(object: I): GameStateResponse {
    const message = createBaseGameStateResponse();
    message.gameState = (object.gameState !== undefined && object.gameState !== null)
      ? GameState.fromPartial(object.gameState)
      : undefined;
    return message;
  },
};

function createBaseFindById(): FindById {
  return { id: "" };
}

export const FindById = {
  encode(message: FindById, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): FindById {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseFindById();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.id = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<FindById, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<FindById | FindById[]> | Iterable<FindById | FindById[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [FindById.encode(p).finish()];
        }
      } else {
        yield* [FindById.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, FindById>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<FindById> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [FindById.decode(p)];
        }
      } else {
        yield* [FindById.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): FindById {
    return { id: isSet(object.id) ? String(object.id) : "" };
  },

  toJSON(message: FindById): unknown {
    const obj: any = {};
    message.id !== undefined && (obj.id = message.id);
    return obj;
  },

  create<I extends Exact<DeepPartial<FindById>, I>>(base?: I): FindById {
    return FindById.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<FindById>, I>>(object: I): FindById {
    const message = createBaseFindById();
    message.id = object.id ?? "";
    return message;
  },
};

export interface Mw5e {
  getGameState(request: UpdateCacheRequest): Promise<GameStateResponse>;
  getCharacter(request: FindById): Promise<Character>;
}

export class Mw5eClientImpl implements Mw5e {
  private readonly rpc: Rpc;
  private readonly service: string;
  constructor(rpc: Rpc, opts?: { service?: string }) {
    this.service = opts?.service || "larp.mw5e.Mw5e";
    this.rpc = rpc;
    this.getGameState = this.getGameState.bind(this);
    this.getCharacter = this.getCharacter.bind(this);
  }
  getGameState(request: UpdateCacheRequest): Promise<GameStateResponse> {
    const data = UpdateCacheRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "GetGameState", data);
    return promise.then((data) => GameStateResponse.decode(new _m0.Reader(data)));
  }

  getCharacter(request: FindById): Promise<Character> {
    const data = FindById.encode(request).finish();
    const promise = this.rpc.request(this.service, "GetCharacter", data);
    return promise.then((data) => Character.decode(new _m0.Reader(data)));
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
