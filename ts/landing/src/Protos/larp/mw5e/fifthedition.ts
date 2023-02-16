/* eslint-disable */
import * as _m0 from "protobufjs/minimal";
import { Gift } from "./gifts";
import { Occupation } from "./occupations";
import { HomeChapter, Religion, Spell, Vantage } from "./other";
import { SkillDefinition } from "./skills";

export const protobufPackage = "larp.mw5e";

export interface GameState {
  lastUpdated: string;
  revision: string;
  gifts: Gift[];
  skills: SkillDefinition[];
  occupations: Occupation[];
  advantages: Vantage[];
  disadvantages: Vantage[];
  religions: Religion[];
  homeChapters: HomeChapter[];
  spells: Spell[];
}

function createBaseGameState(): GameState {
  return {
    lastUpdated: "",
    revision: "",
    gifts: [],
    skills: [],
    occupations: [],
    advantages: [],
    disadvantages: [],
    religions: [],
    homeChapters: [],
    spells: [],
  };
}

export const GameState = {
  encode(message: GameState, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.lastUpdated !== "") {
      writer.uint32(10).string(message.lastUpdated);
    }
    if (message.revision !== "") {
      writer.uint32(18).string(message.revision);
    }
    for (const v of message.gifts) {
      Gift.encode(v!, writer.uint32(82).fork()).ldelim();
    }
    for (const v of message.skills) {
      SkillDefinition.encode(v!, writer.uint32(90).fork()).ldelim();
    }
    for (const v of message.occupations) {
      Occupation.encode(v!, writer.uint32(98).fork()).ldelim();
    }
    for (const v of message.advantages) {
      Vantage.encode(v!, writer.uint32(106).fork()).ldelim();
    }
    for (const v of message.disadvantages) {
      Vantage.encode(v!, writer.uint32(114).fork()).ldelim();
    }
    for (const v of message.religions) {
      Religion.encode(v!, writer.uint32(122).fork()).ldelim();
    }
    for (const v of message.homeChapters) {
      HomeChapter.encode(v!, writer.uint32(130).fork()).ldelim();
    }
    for (const v of message.spells) {
      Spell.encode(v!, writer.uint32(138).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): GameState {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGameState();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.lastUpdated = reader.string();
          break;
        case 2:
          message.revision = reader.string();
          break;
        case 10:
          message.gifts.push(Gift.decode(reader, reader.uint32()));
          break;
        case 11:
          message.skills.push(SkillDefinition.decode(reader, reader.uint32()));
          break;
        case 12:
          message.occupations.push(Occupation.decode(reader, reader.uint32()));
          break;
        case 13:
          message.advantages.push(Vantage.decode(reader, reader.uint32()));
          break;
        case 14:
          message.disadvantages.push(Vantage.decode(reader, reader.uint32()));
          break;
        case 15:
          message.religions.push(Religion.decode(reader, reader.uint32()));
          break;
        case 16:
          message.homeChapters.push(HomeChapter.decode(reader, reader.uint32()));
          break;
        case 17:
          message.spells.push(Spell.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<GameState, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<GameState | GameState[]> | Iterable<GameState | GameState[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GameState.encode(p).finish()];
        }
      } else {
        yield* [GameState.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, GameState>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<GameState> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [GameState.decode(p)];
        }
      } else {
        yield* [GameState.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): GameState {
    return {
      lastUpdated: isSet(object.lastUpdated) ? String(object.lastUpdated) : "",
      revision: isSet(object.revision) ? String(object.revision) : "",
      gifts: Array.isArray(object?.gifts) ? object.gifts.map((e: any) => Gift.fromJSON(e)) : [],
      skills: Array.isArray(object?.skills) ? object.skills.map((e: any) => SkillDefinition.fromJSON(e)) : [],
      occupations: Array.isArray(object?.occupations) ? object.occupations.map((e: any) => Occupation.fromJSON(e)) : [],
      advantages: Array.isArray(object?.advantages) ? object.advantages.map((e: any) => Vantage.fromJSON(e)) : [],
      disadvantages: Array.isArray(object?.disadvantages)
        ? object.disadvantages.map((e: any) => Vantage.fromJSON(e))
        : [],
      religions: Array.isArray(object?.religions) ? object.religions.map((e: any) => Religion.fromJSON(e)) : [],
      homeChapters: Array.isArray(object?.homeChapters)
        ? object.homeChapters.map((e: any) => HomeChapter.fromJSON(e))
        : [],
      spells: Array.isArray(object?.spells) ? object.spells.map((e: any) => Spell.fromJSON(e)) : [],
    };
  },

  toJSON(message: GameState): unknown {
    const obj: any = {};
    message.lastUpdated !== undefined && (obj.lastUpdated = message.lastUpdated);
    message.revision !== undefined && (obj.revision = message.revision);
    if (message.gifts) {
      obj.gifts = message.gifts.map((e) => e ? Gift.toJSON(e) : undefined);
    } else {
      obj.gifts = [];
    }
    if (message.skills) {
      obj.skills = message.skills.map((e) => e ? SkillDefinition.toJSON(e) : undefined);
    } else {
      obj.skills = [];
    }
    if (message.occupations) {
      obj.occupations = message.occupations.map((e) => e ? Occupation.toJSON(e) : undefined);
    } else {
      obj.occupations = [];
    }
    if (message.advantages) {
      obj.advantages = message.advantages.map((e) => e ? Vantage.toJSON(e) : undefined);
    } else {
      obj.advantages = [];
    }
    if (message.disadvantages) {
      obj.disadvantages = message.disadvantages.map((e) => e ? Vantage.toJSON(e) : undefined);
    } else {
      obj.disadvantages = [];
    }
    if (message.religions) {
      obj.religions = message.religions.map((e) => e ? Religion.toJSON(e) : undefined);
    } else {
      obj.religions = [];
    }
    if (message.homeChapters) {
      obj.homeChapters = message.homeChapters.map((e) => e ? HomeChapter.toJSON(e) : undefined);
    } else {
      obj.homeChapters = [];
    }
    if (message.spells) {
      obj.spells = message.spells.map((e) => e ? Spell.toJSON(e) : undefined);
    } else {
      obj.spells = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<GameState>, I>>(base?: I): GameState {
    return GameState.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<GameState>, I>>(object: I): GameState {
    const message = createBaseGameState();
    message.lastUpdated = object.lastUpdated ?? "";
    message.revision = object.revision ?? "";
    message.gifts = object.gifts?.map((e) => Gift.fromPartial(e)) || [];
    message.skills = object.skills?.map((e) => SkillDefinition.fromPartial(e)) || [];
    message.occupations = object.occupations?.map((e) => Occupation.fromPartial(e)) || [];
    message.advantages = object.advantages?.map((e) => Vantage.fromPartial(e)) || [];
    message.disadvantages = object.disadvantages?.map((e) => Vantage.fromPartial(e)) || [];
    message.religions = object.religions?.map((e) => Religion.fromPartial(e)) || [];
    message.homeChapters = object.homeChapters?.map((e) => HomeChapter.fromPartial(e)) || [];
    message.spells = object.spells?.map((e) => Spell.fromPartial(e)) || [];
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
