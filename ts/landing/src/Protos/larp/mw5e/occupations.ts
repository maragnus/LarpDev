/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp.mw5e";

export const OccupationType = {
  OCCUPATION_TYPE_BASIC: 0,
  OCCUPATION_TYPE_YOUTH: 1,
  OCCUPATION_TYPE_ADVANCED: 2,
  OCCUPATION_TYPE_PLOT: 3,
  OCCUPATION_TYPE_ENHANCEMENT: 4,
  UNRECOGNIZED: -1,
} as const;

export type OccupationType = typeof OccupationType[keyof typeof OccupationType];

export function occupationTypeFromJSON(object: any): OccupationType {
  switch (object) {
    case 0:
    case "OCCUPATION_TYPE_BASIC":
      return OccupationType.OCCUPATION_TYPE_BASIC;
    case 1:
    case "OCCUPATION_TYPE_YOUTH":
      return OccupationType.OCCUPATION_TYPE_YOUTH;
    case 2:
    case "OCCUPATION_TYPE_ADVANCED":
      return OccupationType.OCCUPATION_TYPE_ADVANCED;
    case 3:
    case "OCCUPATION_TYPE_PLOT":
      return OccupationType.OCCUPATION_TYPE_PLOT;
    case 4:
    case "OCCUPATION_TYPE_ENHANCEMENT":
      return OccupationType.OCCUPATION_TYPE_ENHANCEMENT;
    case -1:
    case "UNRECOGNIZED":
    default:
      return OccupationType.UNRECOGNIZED;
  }
}

export function occupationTypeToJSON(object: OccupationType): string {
  switch (object) {
    case OccupationType.OCCUPATION_TYPE_BASIC:
      return "OCCUPATION_TYPE_BASIC";
    case OccupationType.OCCUPATION_TYPE_YOUTH:
      return "OCCUPATION_TYPE_YOUTH";
    case OccupationType.OCCUPATION_TYPE_ADVANCED:
      return "OCCUPATION_TYPE_ADVANCED";
    case OccupationType.OCCUPATION_TYPE_PLOT:
      return "OCCUPATION_TYPE_PLOT";
    case OccupationType.OCCUPATION_TYPE_ENHANCEMENT:
      return "OCCUPATION_TYPE_ENHANCEMENT";
    case OccupationType.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

/** Indicates a single skill or a choice of skills */
export interface SkillChoice {
  count: number;
  choices: string[];
}

export interface Occupation {
  name: string;
  specialties: string[];
  type: string;
  skills: string[];
  choices: SkillChoice[];
  duty?: string | undefined;
  livery?: string | undefined;
}

function createBaseSkillChoice(): SkillChoice {
  return { count: 0, choices: [] };
}

export const SkillChoice = {
  encode(message: SkillChoice, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.count !== 0) {
      writer.uint32(8).int32(message.count);
    }
    for (const v of message.choices) {
      writer.uint32(18).string(v!);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): SkillChoice {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseSkillChoice();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.count = reader.int32();
          break;
        case 2:
          message.choices.push(reader.string());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<SkillChoice, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<SkillChoice | SkillChoice[]> | Iterable<SkillChoice | SkillChoice[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [SkillChoice.encode(p).finish()];
        }
      } else {
        yield* [SkillChoice.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, SkillChoice>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<SkillChoice> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [SkillChoice.decode(p)];
        }
      } else {
        yield* [SkillChoice.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): SkillChoice {
    return {
      count: isSet(object.count) ? Number(object.count) : 0,
      choices: Array.isArray(object?.choices) ? object.choices.map((e: any) => String(e)) : [],
    };
  },

  toJSON(message: SkillChoice): unknown {
    const obj: any = {};
    message.count !== undefined && (obj.count = Math.round(message.count));
    if (message.choices) {
      obj.choices = message.choices.map((e) => e);
    } else {
      obj.choices = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<SkillChoice>, I>>(base?: I): SkillChoice {
    return SkillChoice.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<SkillChoice>, I>>(object: I): SkillChoice {
    const message = createBaseSkillChoice();
    message.count = object.count ?? 0;
    message.choices = object.choices?.map((e) => e) || [];
    return message;
  },
};

function createBaseOccupation(): Occupation {
  return { name: "", specialties: [], type: "", skills: [], choices: [], duty: undefined, livery: undefined };
}

export const Occupation = {
  encode(message: Occupation, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    for (const v of message.specialties) {
      writer.uint32(18).string(v!);
    }
    if (message.type !== "") {
      writer.uint32(26).string(message.type);
    }
    for (const v of message.skills) {
      writer.uint32(34).string(v!);
    }
    for (const v of message.choices) {
      SkillChoice.encode(v!, writer.uint32(42).fork()).ldelim();
    }
    if (message.duty !== undefined) {
      writer.uint32(50).string(message.duty);
    }
    if (message.livery !== undefined) {
      writer.uint32(58).string(message.livery);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Occupation {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseOccupation();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.specialties.push(reader.string());
          break;
        case 3:
          message.type = reader.string();
          break;
        case 4:
          message.skills.push(reader.string());
          break;
        case 5:
          message.choices.push(SkillChoice.decode(reader, reader.uint32()));
          break;
        case 6:
          message.duty = reader.string();
          break;
        case 7:
          message.livery = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Occupation, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Occupation | Occupation[]> | Iterable<Occupation | Occupation[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Occupation.encode(p).finish()];
        }
      } else {
        yield* [Occupation.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Occupation>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Occupation> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Occupation.decode(p)];
        }
      } else {
        yield* [Occupation.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Occupation {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      specialties: Array.isArray(object?.specialties) ? object.specialties.map((e: any) => String(e)) : [],
      type: isSet(object.type) ? String(object.type) : "",
      skills: Array.isArray(object?.skills) ? object.skills.map((e: any) => String(e)) : [],
      choices: Array.isArray(object?.choices) ? object.choices.map((e: any) => SkillChoice.fromJSON(e)) : [],
      duty: isSet(object.duty) ? String(object.duty) : undefined,
      livery: isSet(object.livery) ? String(object.livery) : undefined,
    };
  },

  toJSON(message: Occupation): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    if (message.specialties) {
      obj.specialties = message.specialties.map((e) => e);
    } else {
      obj.specialties = [];
    }
    message.type !== undefined && (obj.type = message.type);
    if (message.skills) {
      obj.skills = message.skills.map((e) => e);
    } else {
      obj.skills = [];
    }
    if (message.choices) {
      obj.choices = message.choices.map((e) => e ? SkillChoice.toJSON(e) : undefined);
    } else {
      obj.choices = [];
    }
    message.duty !== undefined && (obj.duty = message.duty);
    message.livery !== undefined && (obj.livery = message.livery);
    return obj;
  },

  create<I extends Exact<DeepPartial<Occupation>, I>>(base?: I): Occupation {
    return Occupation.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Occupation>, I>>(object: I): Occupation {
    const message = createBaseOccupation();
    message.name = object.name ?? "";
    message.specialties = object.specialties?.map((e) => e) || [];
    message.type = object.type ?? "";
    message.skills = object.skills?.map((e) => e) || [];
    message.choices = object.choices?.map((e) => SkillChoice.fromPartial(e)) || [];
    message.duty = object.duty ?? undefined;
    message.livery = object.livery ?? undefined;
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
