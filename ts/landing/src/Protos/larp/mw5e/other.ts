/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp.mw5e";

export const SpellType = {
  SPELL_TYPE_BOLT: 0,
  SPELL_TYPE_GESTURE: 1,
  SPELL_TYPE_SPRAY: 2,
  SPELL_TYPE_VOICE: 3,
  SPELL_TYPE_STORM: 4,
  SPELL_TYPE_ROOM: 5,
  SPELL_TYPE_GESTURE_OR_VOICE: 6,
  UNRECOGNIZED: -1,
} as const;

export type SpellType = typeof SpellType[keyof typeof SpellType];

export function spellTypeFromJSON(object: any): SpellType {
  switch (object) {
    case 0:
    case "SPELL_TYPE_BOLT":
      return SpellType.SPELL_TYPE_BOLT;
    case 1:
    case "SPELL_TYPE_GESTURE":
      return SpellType.SPELL_TYPE_GESTURE;
    case 2:
    case "SPELL_TYPE_SPRAY":
      return SpellType.SPELL_TYPE_SPRAY;
    case 3:
    case "SPELL_TYPE_VOICE":
      return SpellType.SPELL_TYPE_VOICE;
    case 4:
    case "SPELL_TYPE_STORM":
      return SpellType.SPELL_TYPE_STORM;
    case 5:
    case "SPELL_TYPE_ROOM":
      return SpellType.SPELL_TYPE_ROOM;
    case 6:
    case "SPELL_TYPE_GESTURE_OR_VOICE":
      return SpellType.SPELL_TYPE_GESTURE_OR_VOICE;
    case -1:
    case "UNRECOGNIZED":
    default:
      return SpellType.UNRECOGNIZED;
  }
}

export function spellTypeToJSON(object: SpellType): string {
  switch (object) {
    case SpellType.SPELL_TYPE_BOLT:
      return "SPELL_TYPE_BOLT";
    case SpellType.SPELL_TYPE_GESTURE:
      return "SPELL_TYPE_GESTURE";
    case SpellType.SPELL_TYPE_SPRAY:
      return "SPELL_TYPE_SPRAY";
    case SpellType.SPELL_TYPE_VOICE:
      return "SPELL_TYPE_VOICE";
    case SpellType.SPELL_TYPE_STORM:
      return "SPELL_TYPE_STORM";
    case SpellType.SPELL_TYPE_ROOM:
      return "SPELL_TYPE_ROOM";
    case SpellType.SPELL_TYPE_GESTURE_OR_VOICE:
      return "SPELL_TYPE_GESTURE_OR_VOICE";
    case SpellType.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface Vantage {
  name: string;
  title: string;
  rank: number;
  physical: boolean;
}

export interface Religion {
  name: string;
  title: string;
}

export interface HomeChapter {
  name: string;
  title: string;
}

export interface Spell {
  name: string;
  type: string;
  category: string;
  mana: number;
  effect: string;
}

function createBaseVantage(): Vantage {
  return { name: "", title: "", rank: 0, physical: false };
}

export const Vantage = {
  encode(message: Vantage, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    if (message.rank !== 0) {
      writer.uint32(24).int32(message.rank);
    }
    if (message.physical === true) {
      writer.uint32(32).bool(message.physical);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Vantage {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseVantage();
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
          message.rank = reader.int32();
          break;
        case 4:
          message.physical = reader.bool();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Vantage, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Vantage | Vantage[]> | Iterable<Vantage | Vantage[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Vantage.encode(p).finish()];
        }
      } else {
        yield* [Vantage.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Vantage>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Vantage> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Vantage.decode(p)];
        }
      } else {
        yield* [Vantage.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Vantage {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
      rank: isSet(object.rank) ? Number(object.rank) : 0,
      physical: isSet(object.physical) ? Boolean(object.physical) : false,
    };
  },

  toJSON(message: Vantage): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    message.rank !== undefined && (obj.rank = Math.round(message.rank));
    message.physical !== undefined && (obj.physical = message.physical);
    return obj;
  },

  create<I extends Exact<DeepPartial<Vantage>, I>>(base?: I): Vantage {
    return Vantage.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Vantage>, I>>(object: I): Vantage {
    const message = createBaseVantage();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    message.rank = object.rank ?? 0;
    message.physical = object.physical ?? false;
    return message;
  },
};

function createBaseReligion(): Religion {
  return { name: "", title: "" };
}

export const Religion = {
  encode(message: Religion, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Religion {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseReligion();
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
  // Transform<Religion, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Religion | Religion[]> | Iterable<Religion | Religion[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Religion.encode(p).finish()];
        }
      } else {
        yield* [Religion.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Religion>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Religion> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Religion.decode(p)];
        }
      } else {
        yield* [Religion.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Religion {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
    };
  },

  toJSON(message: Religion): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    return obj;
  },

  create<I extends Exact<DeepPartial<Religion>, I>>(base?: I): Religion {
    return Religion.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Religion>, I>>(object: I): Religion {
    const message = createBaseReligion();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    return message;
  },
};

function createBaseHomeChapter(): HomeChapter {
  return { name: "", title: "" };
}

export const HomeChapter = {
  encode(message: HomeChapter, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): HomeChapter {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseHomeChapter();
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
  // Transform<HomeChapter, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<HomeChapter | HomeChapter[]> | Iterable<HomeChapter | HomeChapter[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [HomeChapter.encode(p).finish()];
        }
      } else {
        yield* [HomeChapter.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, HomeChapter>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<HomeChapter> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [HomeChapter.decode(p)];
        }
      } else {
        yield* [HomeChapter.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): HomeChapter {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
    };
  },

  toJSON(message: HomeChapter): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    return obj;
  },

  create<I extends Exact<DeepPartial<HomeChapter>, I>>(base?: I): HomeChapter {
    return HomeChapter.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<HomeChapter>, I>>(object: I): HomeChapter {
    const message = createBaseHomeChapter();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    return message;
  },
};

function createBaseSpell(): Spell {
  return { name: "", type: "", category: "", mana: 0, effect: "" };
}

export const Spell = {
  encode(message: Spell, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.type !== "") {
      writer.uint32(18).string(message.type);
    }
    if (message.category !== "") {
      writer.uint32(26).string(message.category);
    }
    if (message.mana !== 0) {
      writer.uint32(32).int32(message.mana);
    }
    if (message.effect !== "") {
      writer.uint32(42).string(message.effect);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Spell {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseSpell();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.type = reader.string();
          break;
        case 3:
          message.category = reader.string();
          break;
        case 4:
          message.mana = reader.int32();
          break;
        case 5:
          message.effect = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Spell, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Spell | Spell[]> | Iterable<Spell | Spell[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Spell.encode(p).finish()];
        }
      } else {
        yield* [Spell.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Spell>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Spell> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Spell.decode(p)];
        }
      } else {
        yield* [Spell.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Spell {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      type: isSet(object.type) ? String(object.type) : "",
      category: isSet(object.category) ? String(object.category) : "",
      mana: isSet(object.mana) ? Number(object.mana) : 0,
      effect: isSet(object.effect) ? String(object.effect) : "",
    };
  },

  toJSON(message: Spell): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.type !== undefined && (obj.type = message.type);
    message.category !== undefined && (obj.category = message.category);
    message.mana !== undefined && (obj.mana = Math.round(message.mana));
    message.effect !== undefined && (obj.effect = message.effect);
    return obj;
  },

  create<I extends Exact<DeepPartial<Spell>, I>>(base?: I): Spell {
    return Spell.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Spell>, I>>(object: I): Spell {
    const message = createBaseSpell();
    message.name = object.name ?? "";
    message.type = object.type ?? "";
    message.category = object.category ?? "";
    message.mana = object.mana ?? 0;
    message.effect = object.effect ?? "";
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
