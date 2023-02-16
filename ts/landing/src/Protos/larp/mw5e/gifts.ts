/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp.mw5e";

export interface Gift {
  name: string;
  title: string;
  properties: string[];
  ranks: GiftRank[];
}

export interface GiftProperty {
  name: string;
  title: string;
}

export interface GiftPropertyValue {
  name: string;
  value: string;
}

export interface GiftRank {
  rank: number;
  properties: string[];
  abilities: Ability[];
}

export interface Ability {
  /** Name without rank */
  name: string;
  /** 0 if there's only one rank, otherwise represents I, II, IV, 1, 2 ,3 */
  rank: number;
  /** Name and Rank verbatim */
  title: string;
}

function createBaseGift(): Gift {
  return { name: "", title: "", properties: [], ranks: [] };
}

export const Gift = {
  encode(message: Gift, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    for (const v of message.properties) {
      writer.uint32(26).string(v!);
    }
    for (const v of message.ranks) {
      GiftRank.encode(v!, writer.uint32(34).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Gift {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGift();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.title = reader.string();
          break;
        case 3:
          message.properties.push(reader.string());
          break;
        case 4:
          message.ranks.push(GiftRank.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Gift, Uint8Array>
  async *encodeTransform(source: AsyncIterable<Gift | Gift[]> | Iterable<Gift | Gift[]>): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Gift.encode(p).finish()];
        }
      } else {
        yield* [Gift.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Gift>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Gift> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Gift.decode(p)];
        }
      } else {
        yield* [Gift.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Gift {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
      properties: Array.isArray(object?.properties) ? object.properties.map((e: any) => String(e)) : [],
      ranks: Array.isArray(object?.ranks) ? object.ranks.map((e: any) => GiftRank.fromJSON(e)) : [],
    };
  },

  toJSON(message: Gift): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    if (message.properties) {
      obj.properties = message.properties.map((e) => e);
    } else {
      obj.properties = [];
    }
    if (message.ranks) {
      obj.ranks = message.ranks.map((e) => e ? GiftRank.toJSON(e) : undefined);
    } else {
      obj.ranks = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<Gift>, I>>(base?: I): Gift {
    return Gift.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Gift>, I>>(object: I): Gift {
    const message = createBaseGift();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    message.properties = object.properties?.map((e) => e) || [];
    message.ranks = object.ranks?.map((e) => GiftRank.fromPartial(e)) || [];
    return message;
  },
};

function createBaseGiftProperty(): GiftProperty {
  return { name: "", title: "" };
}

export const GiftProperty = {
  encode(message: GiftProperty, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): GiftProperty {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGiftProperty();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.title = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<GiftProperty, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<GiftProperty | GiftProperty[]> | Iterable<GiftProperty | GiftProperty[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftProperty.encode(p).finish()];
        }
      } else {
        yield* [GiftProperty.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, GiftProperty>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<GiftProperty> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftProperty.decode(p)];
        }
      } else {
        yield* [GiftProperty.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): GiftProperty {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
    };
  },

  toJSON(message: GiftProperty): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    return obj;
  },

  create<I extends Exact<DeepPartial<GiftProperty>, I>>(base?: I): GiftProperty {
    return GiftProperty.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<GiftProperty>, I>>(object: I): GiftProperty {
    const message = createBaseGiftProperty();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    return message;
  },
};

function createBaseGiftPropertyValue(): GiftPropertyValue {
  return { name: "", value: "" };
}

export const GiftPropertyValue = {
  encode(message: GiftPropertyValue, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.value !== "") {
      writer.uint32(18).string(message.value);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): GiftPropertyValue {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGiftPropertyValue();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.value = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<GiftPropertyValue, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<GiftPropertyValue | GiftPropertyValue[]> | Iterable<GiftPropertyValue | GiftPropertyValue[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftPropertyValue.encode(p).finish()];
        }
      } else {
        yield* [GiftPropertyValue.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, GiftPropertyValue>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<GiftPropertyValue> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftPropertyValue.decode(p)];
        }
      } else {
        yield* [GiftPropertyValue.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): GiftPropertyValue {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      value: isSet(object.value) ? String(object.value) : "",
    };
  },

  toJSON(message: GiftPropertyValue): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.value !== undefined && (obj.value = message.value);
    return obj;
  },

  create<I extends Exact<DeepPartial<GiftPropertyValue>, I>>(base?: I): GiftPropertyValue {
    return GiftPropertyValue.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<GiftPropertyValue>, I>>(object: I): GiftPropertyValue {
    const message = createBaseGiftPropertyValue();
    message.name = object.name ?? "";
    message.value = object.value ?? "";
    return message;
  },
};

function createBaseGiftRank(): GiftRank {
  return { rank: 0, properties: [], abilities: [] };
}

export const GiftRank = {
  encode(message: GiftRank, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.rank !== 0) {
      writer.uint32(8).int32(message.rank);
    }
    for (const v of message.properties) {
      writer.uint32(18).string(v!);
    }
    for (const v of message.abilities) {
      Ability.encode(v!, writer.uint32(26).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): GiftRank {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGiftRank();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.rank = reader.int32();
          break;
        case 2:
          message.properties.push(reader.string());
          break;
        case 3:
          message.abilities.push(Ability.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<GiftRank, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<GiftRank | GiftRank[]> | Iterable<GiftRank | GiftRank[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftRank.encode(p).finish()];
        }
      } else {
        yield* [GiftRank.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, GiftRank>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<GiftRank> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GiftRank.decode(p)];
        }
      } else {
        yield* [GiftRank.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): GiftRank {
    return {
      rank: isSet(object.rank) ? Number(object.rank) : 0,
      properties: Array.isArray(object?.properties) ? object.properties.map((e: any) => String(e)) : [],
      abilities: Array.isArray(object?.abilities) ? object.abilities.map((e: any) => Ability.fromJSON(e)) : [],
    };
  },

  toJSON(message: GiftRank): unknown {
    const obj: any = {};
    message.rank !== undefined && (obj.rank = Math.round(message.rank));
    if (message.properties) {
      obj.properties = message.properties.map((e) => e);
    } else {
      obj.properties = [];
    }
    if (message.abilities) {
      obj.abilities = message.abilities.map((e) => e ? Ability.toJSON(e) : undefined);
    } else {
      obj.abilities = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<GiftRank>, I>>(base?: I): GiftRank {
    return GiftRank.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<GiftRank>, I>>(object: I): GiftRank {
    const message = createBaseGiftRank();
    message.rank = object.rank ?? 0;
    message.properties = object.properties?.map((e) => e) || [];
    message.abilities = object.abilities?.map((e) => Ability.fromPartial(e)) || [];
    return message;
  },
};

function createBaseAbility(): Ability {
  return { name: "", rank: 0, title: "" };
}

export const Ability = {
  encode(message: Ability, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.rank !== 0) {
      writer.uint32(16).int32(message.rank);
    }
    if (message.title !== "") {
      writer.uint32(26).string(message.title);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Ability {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAbility();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.rank = reader.int32();
          break;
        case 3:
          message.title = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Ability, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Ability | Ability[]> | Iterable<Ability | Ability[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Ability.encode(p).finish()];
        }
      } else {
        yield* [Ability.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Ability>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Ability> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Ability.decode(p)];
        }
      } else {
        yield* [Ability.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Ability {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      rank: isSet(object.rank) ? Number(object.rank) : 0,
      title: isSet(object.title) ? String(object.title) : "",
    };
  },

  toJSON(message: Ability): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.rank !== undefined && (obj.rank = Math.round(message.rank));
    message.title !== undefined && (obj.title = message.title);
    return obj;
  },

  create<I extends Exact<DeepPartial<Ability>, I>>(base?: I): Ability {
    return Ability.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Ability>, I>>(object: I): Ability {
    const message = createBaseAbility();
    message.name = object.name ?? "";
    message.rank = object.rank ?? 0;
    message.title = object.title ?? "";
    return message;
  },
};

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
