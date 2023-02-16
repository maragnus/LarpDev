/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp.mw5e";

export const SkillClass = {
  SKILL_CLASS_UNAVAILABLE: 0,
  SKILL_CLASS_FREE: 1,
  SKILL_CLASS_MINOR: 2,
  SKILL_CLASS_STANDARD: 3,
  SKILL_CLASS_MAJOR: 4,
  UNRECOGNIZED: -1,
} as const;

export type SkillClass = typeof SkillClass[keyof typeof SkillClass];

export function skillClassFromJSON(object: any): SkillClass {
  switch (object) {
    case 0:
    case "SKILL_CLASS_UNAVAILABLE":
      return SkillClass.SKILL_CLASS_UNAVAILABLE;
    case 1:
    case "SKILL_CLASS_FREE":
      return SkillClass.SKILL_CLASS_FREE;
    case 2:
    case "SKILL_CLASS_MINOR":
      return SkillClass.SKILL_CLASS_MINOR;
    case 3:
    case "SKILL_CLASS_STANDARD":
      return SkillClass.SKILL_CLASS_STANDARD;
    case 4:
    case "SKILL_CLASS_MAJOR":
      return SkillClass.SKILL_CLASS_MAJOR;
    case -1:
    case "UNRECOGNIZED":
    default:
      return SkillClass.UNRECOGNIZED;
  }
}

export function skillClassToJSON(object: SkillClass): string {
  switch (object) {
    case SkillClass.SKILL_CLASS_UNAVAILABLE:
      return "SKILL_CLASS_UNAVAILABLE";
    case SkillClass.SKILL_CLASS_FREE:
      return "SKILL_CLASS_FREE";
    case SkillClass.SKILL_CLASS_MINOR:
      return "SKILL_CLASS_MINOR";
    case SkillClass.SKILL_CLASS_STANDARD:
      return "SKILL_CLASS_STANDARD";
    case SkillClass.SKILL_CLASS_MAJOR:
      return "SKILL_CLASS_MAJOR";
    case SkillClass.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export const SkillPurchasable = {
  SKILL_PURCHASABLE_UNAVAILABLE: 0,
  SKILL_PURCHASABLE_ONCE: 1,
  SKILL_PURCHASABLE_MULTIPLE: 2,
  UNRECOGNIZED: -1,
} as const;

export type SkillPurchasable = typeof SkillPurchasable[keyof typeof SkillPurchasable];

export function skillPurchasableFromJSON(object: any): SkillPurchasable {
  switch (object) {
    case 0:
    case "SKILL_PURCHASABLE_UNAVAILABLE":
      return SkillPurchasable.SKILL_PURCHASABLE_UNAVAILABLE;
    case 1:
    case "SKILL_PURCHASABLE_ONCE":
      return SkillPurchasable.SKILL_PURCHASABLE_ONCE;
    case 2:
    case "SKILL_PURCHASABLE_MULTIPLE":
      return SkillPurchasable.SKILL_PURCHASABLE_MULTIPLE;
    case -1:
    case "UNRECOGNIZED":
    default:
      return SkillPurchasable.UNRECOGNIZED;
  }
}

export function skillPurchasableToJSON(object: SkillPurchasable): string {
  switch (object) {
    case SkillPurchasable.SKILL_PURCHASABLE_UNAVAILABLE:
      return "SKILL_PURCHASABLE_UNAVAILABLE";
    case SkillPurchasable.SKILL_PURCHASABLE_ONCE:
      return "SKILL_PURCHASABLE_ONCE";
    case SkillPurchasable.SKILL_PURCHASABLE_MULTIPLE:
      return "SKILL_PURCHASABLE_MULTIPLE";
    case SkillPurchasable.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface SkillDefinition {
  name: string;
  title: string;
  class: string;
  purchasable: string;
  ranksPerPurchase?: number | undefined;
  costPerPurchase?: number | undefined;
  iterations: string[];
}

function createBaseSkillDefinition(): SkillDefinition {
  return {
    name: "",
    title: "",
    class: "",
    purchasable: "",
    ranksPerPurchase: undefined,
    costPerPurchase: undefined,
    iterations: [],
  };
}

export const SkillDefinition = {
  encode(message: SkillDefinition, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.title !== "") {
      writer.uint32(18).string(message.title);
    }
    if (message.class !== "") {
      writer.uint32(26).string(message.class);
    }
    if (message.purchasable !== "") {
      writer.uint32(34).string(message.purchasable);
    }
    if (message.ranksPerPurchase !== undefined) {
      writer.uint32(40).int32(message.ranksPerPurchase);
    }
    if (message.costPerPurchase !== undefined) {
      writer.uint32(48).int32(message.costPerPurchase);
    }
    for (const v of message.iterations) {
      writer.uint32(58).string(v!);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): SkillDefinition {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseSkillDefinition();
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
          message.class = reader.string();
          break;
        case 4:
          message.purchasable = reader.string();
          break;
        case 5:
          message.ranksPerPurchase = reader.int32();
          break;
        case 6:
          message.costPerPurchase = reader.int32();
          break;
        case 7:
          message.iterations.push(reader.string());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<SkillDefinition, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<SkillDefinition | SkillDefinition[]> | Iterable<SkillDefinition | SkillDefinition[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [SkillDefinition.encode(p).finish()];
        }
      } else {
        yield* [SkillDefinition.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, SkillDefinition>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<SkillDefinition> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [SkillDefinition.decode(p)];
        }
      } else {
        yield* [SkillDefinition.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): SkillDefinition {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      title: isSet(object.title) ? String(object.title) : "",
      class: isSet(object.class) ? String(object.class) : "",
      purchasable: isSet(object.purchasable) ? String(object.purchasable) : "",
      ranksPerPurchase: isSet(object.ranksPerPurchase) ? Number(object.ranksPerPurchase) : undefined,
      costPerPurchase: isSet(object.costPerPurchase) ? Number(object.costPerPurchase) : undefined,
      iterations: Array.isArray(object?.iterations) ? object.iterations.map((e: any) => String(e)) : [],
    };
  },

  toJSON(message: SkillDefinition): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.title !== undefined && (obj.title = message.title);
    message.class !== undefined && (obj.class = message.class);
    message.purchasable !== undefined && (obj.purchasable = message.purchasable);
    message.ranksPerPurchase !== undefined && (obj.ranksPerPurchase = Math.round(message.ranksPerPurchase));
    message.costPerPurchase !== undefined && (obj.costPerPurchase = Math.round(message.costPerPurchase));
    if (message.iterations) {
      obj.iterations = message.iterations.map((e) => e);
    } else {
      obj.iterations = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<SkillDefinition>, I>>(base?: I): SkillDefinition {
    return SkillDefinition.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<SkillDefinition>, I>>(object: I): SkillDefinition {
    const message = createBaseSkillDefinition();
    message.name = object.name ?? "";
    message.title = object.title ?? "";
    message.class = object.class ?? "";
    message.purchasable = object.purchasable ?? "";
    message.ranksPerPurchase = object.ranksPerPurchase ?? undefined;
    message.costPerPurchase = object.costPerPurchase ?? undefined;
    message.iterations = object.iterations?.map((e) => e) || [];
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
