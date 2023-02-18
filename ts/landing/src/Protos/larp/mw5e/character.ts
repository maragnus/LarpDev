/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp.mw5e";

export const SkillPurchase = {
  SKILL_PURCHASE_FREE: 0,
  SKILL_PURCHASE_OCCUPATION: 1,
  SKILL_PURCHASE_PURCHASE: 2,
  SKILL_PURCHASE_BESTOWED: 3,
  UNRECOGNIZED: -1,
} as const;

export type SkillPurchase = typeof SkillPurchase[keyof typeof SkillPurchase];

export function skillPurchaseFromJSON(object: any): SkillPurchase {
  switch (object) {
    case 0:
    case "SKILL_PURCHASE_FREE":
      return SkillPurchase.SKILL_PURCHASE_FREE;
    case 1:
    case "SKILL_PURCHASE_OCCUPATION":
      return SkillPurchase.SKILL_PURCHASE_OCCUPATION;
    case 2:
    case "SKILL_PURCHASE_PURCHASE":
      return SkillPurchase.SKILL_PURCHASE_PURCHASE;
    case 3:
    case "SKILL_PURCHASE_BESTOWED":
      return SkillPurchase.SKILL_PURCHASE_BESTOWED;
    case -1:
    case "UNRECOGNIZED":
    default:
      return SkillPurchase.UNRECOGNIZED;
  }
}

export function skillPurchaseToJSON(object: SkillPurchase): string {
  switch (object) {
    case SkillPurchase.SKILL_PURCHASE_FREE:
      return "SKILL_PURCHASE_FREE";
    case SkillPurchase.SKILL_PURCHASE_OCCUPATION:
      return "SKILL_PURCHASE_OCCUPATION";
    case SkillPurchase.SKILL_PURCHASE_PURCHASE:
      return "SKILL_PURCHASE_PURCHASE";
    case SkillPurchase.SKILL_PURCHASE_BESTOWED:
      return "SKILL_PURCHASE_BESTOWED";
    case SkillPurchase.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface CharacterSkill {
  name: string;
  rank: number;
  type: SkillPurchase;
  purchases?: number | undefined;
}

export interface CharacterVantage {
  name: string;
  rank: number;
}

export interface Character {
  accountId: string;
  characterName: string;
  religions: string;
  occupation: string;
  specialty: string;
  enhancement: string;
  homeChapter: string;
  publicStory: string;
  privateStory: string;
  homeland: string;
  startingMoonstone: number;
  skillTokens: number;
  courage: number;
  dexterity: number;
  empathy: number;
  passion: number;
  prowess: number;
  wisdom: number;
  skills: CharacterSkill[];
  advantages: CharacterVantage[];
  disadvantages: CharacterVantage[];
  spells: string[];
  flavorTraits: string[];
  unusualFeatures?: string | undefined;
  cures?: string | undefined;
  documents?: string | undefined;
  notes?: string | undefined;
}

function createBaseCharacterSkill(): CharacterSkill {
  return { name: "", rank: 0, type: 0, purchases: undefined };
}

export const CharacterSkill = {
  encode(message: CharacterSkill, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.rank !== 0) {
      writer.uint32(16).int32(message.rank);
    }
    if (message.type !== 0) {
      writer.uint32(24).int32(message.type);
    }
    if (message.purchases !== undefined) {
      writer.uint32(32).int32(message.purchases);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): CharacterSkill {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCharacterSkill();
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
          message.type = reader.int32() as any;
          break;
        case 4:
          message.purchases = reader.int32();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<CharacterSkill, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<CharacterSkill | CharacterSkill[]> | Iterable<CharacterSkill | CharacterSkill[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [CharacterSkill.encode(p).finish()];
        }
      } else {
        yield* [CharacterSkill.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, CharacterSkill>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<CharacterSkill> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [CharacterSkill.decode(p)];
        }
      } else {
        yield* [CharacterSkill.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): CharacterSkill {
    return {
      name: isSet(object.name) ? String(object.name) : "",
      rank: isSet(object.rank) ? Number(object.rank) : 0,
      type: isSet(object.type) ? skillPurchaseFromJSON(object.type) : 0,
      purchases: isSet(object.purchases) ? Number(object.purchases) : undefined,
    };
  },

  toJSON(message: CharacterSkill): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.rank !== undefined && (obj.rank = Math.round(message.rank));
    message.type !== undefined && (obj.type = skillPurchaseToJSON(message.type));
    message.purchases !== undefined && (obj.purchases = Math.round(message.purchases));
    return obj;
  },

  create<I extends Exact<DeepPartial<CharacterSkill>, I>>(base?: I): CharacterSkill {
    return CharacterSkill.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<CharacterSkill>, I>>(object: I): CharacterSkill {
    const message = createBaseCharacterSkill();
    message.name = object.name ?? "";
    message.rank = object.rank ?? 0;
    message.type = object.type ?? 0;
    message.purchases = object.purchases ?? undefined;
    return message;
  },
};

function createBaseCharacterVantage(): CharacterVantage {
  return { name: "", rank: 0 };
}

export const CharacterVantage = {
  encode(message: CharacterVantage, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.rank !== 0) {
      writer.uint32(16).int32(message.rank);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): CharacterVantage {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCharacterVantage();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.rank = reader.int32();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<CharacterVantage, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<CharacterVantage | CharacterVantage[]> | Iterable<CharacterVantage | CharacterVantage[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [CharacterVantage.encode(p).finish()];
        }
      } else {
        yield* [CharacterVantage.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, CharacterVantage>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<CharacterVantage> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [CharacterVantage.decode(p)];
        }
      } else {
        yield* [CharacterVantage.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): CharacterVantage {
    return { name: isSet(object.name) ? String(object.name) : "", rank: isSet(object.rank) ? Number(object.rank) : 0 };
  },

  toJSON(message: CharacterVantage): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.rank !== undefined && (obj.rank = Math.round(message.rank));
    return obj;
  },

  create<I extends Exact<DeepPartial<CharacterVantage>, I>>(base?: I): CharacterVantage {
    return CharacterVantage.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<CharacterVantage>, I>>(object: I): CharacterVantage {
    const message = createBaseCharacterVantage();
    message.name = object.name ?? "";
    message.rank = object.rank ?? 0;
    return message;
  },
};

function createBaseCharacter(): Character {
  return {
    accountId: "",
    characterName: "",
    religions: "",
    occupation: "",
    specialty: "",
    enhancement: "",
    homeChapter: "",
    publicStory: "",
    privateStory: "",
    homeland: "",
    startingMoonstone: 0,
    skillTokens: 0,
    courage: 0,
    dexterity: 0,
    empathy: 0,
    passion: 0,
    prowess: 0,
    wisdom: 0,
    skills: [],
    advantages: [],
    disadvantages: [],
    spells: [],
    flavorTraits: [],
    unusualFeatures: undefined,
    cures: undefined,
    documents: undefined,
    notes: undefined,
  };
}

export const Character = {
  encode(message: Character, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.accountId !== "") {
      writer.uint32(10).string(message.accountId);
    }
    if (message.characterName !== "") {
      writer.uint32(18).string(message.characterName);
    }
    if (message.religions !== "") {
      writer.uint32(26).string(message.religions);
    }
    if (message.occupation !== "") {
      writer.uint32(34).string(message.occupation);
    }
    if (message.specialty !== "") {
      writer.uint32(42).string(message.specialty);
    }
    if (message.enhancement !== "") {
      writer.uint32(50).string(message.enhancement);
    }
    if (message.homeChapter !== "") {
      writer.uint32(58).string(message.homeChapter);
    }
    if (message.publicStory !== "") {
      writer.uint32(66).string(message.publicStory);
    }
    if (message.privateStory !== "") {
      writer.uint32(74).string(message.privateStory);
    }
    if (message.homeland !== "") {
      writer.uint32(82).string(message.homeland);
    }
    if (message.startingMoonstone !== 0) {
      writer.uint32(88).int32(message.startingMoonstone);
    }
    if (message.skillTokens !== 0) {
      writer.uint32(96).int32(message.skillTokens);
    }
    if (message.courage !== 0) {
      writer.uint32(160).int32(message.courage);
    }
    if (message.dexterity !== 0) {
      writer.uint32(168).int32(message.dexterity);
    }
    if (message.empathy !== 0) {
      writer.uint32(176).int32(message.empathy);
    }
    if (message.passion !== 0) {
      writer.uint32(184).int32(message.passion);
    }
    if (message.prowess !== 0) {
      writer.uint32(192).int32(message.prowess);
    }
    if (message.wisdom !== 0) {
      writer.uint32(200).int32(message.wisdom);
    }
    for (const v of message.skills) {
      CharacterSkill.encode(v!, writer.uint32(210).fork()).ldelim();
    }
    for (const v of message.advantages) {
      CharacterVantage.encode(v!, writer.uint32(218).fork()).ldelim();
    }
    for (const v of message.disadvantages) {
      CharacterVantage.encode(v!, writer.uint32(226).fork()).ldelim();
    }
    for (const v of message.spells) {
      writer.uint32(234).string(v!);
    }
    for (const v of message.flavorTraits) {
      writer.uint32(242).string(v!);
    }
    if (message.unusualFeatures !== undefined) {
      writer.uint32(250).string(message.unusualFeatures);
    }
    if (message.cures !== undefined) {
      writer.uint32(258).string(message.cures);
    }
    if (message.documents !== undefined) {
      writer.uint32(266).string(message.documents);
    }
    if (message.notes !== undefined) {
      writer.uint32(274).string(message.notes);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Character {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCharacter();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.accountId = reader.string();
          break;
        case 2:
          message.characterName = reader.string();
          break;
        case 3:
          message.religions = reader.string();
          break;
        case 4:
          message.occupation = reader.string();
          break;
        case 5:
          message.specialty = reader.string();
          break;
        case 6:
          message.enhancement = reader.string();
          break;
        case 7:
          message.homeChapter = reader.string();
          break;
        case 8:
          message.publicStory = reader.string();
          break;
        case 9:
          message.privateStory = reader.string();
          break;
        case 10:
          message.homeland = reader.string();
          break;
        case 11:
          message.startingMoonstone = reader.int32();
          break;
        case 12:
          message.skillTokens = reader.int32();
          break;
        case 20:
          message.courage = reader.int32();
          break;
        case 21:
          message.dexterity = reader.int32();
          break;
        case 22:
          message.empathy = reader.int32();
          break;
        case 23:
          message.passion = reader.int32();
          break;
        case 24:
          message.prowess = reader.int32();
          break;
        case 25:
          message.wisdom = reader.int32();
          break;
        case 26:
          message.skills.push(CharacterSkill.decode(reader, reader.uint32()));
          break;
        case 27:
          message.advantages.push(CharacterVantage.decode(reader, reader.uint32()));
          break;
        case 28:
          message.disadvantages.push(CharacterVantage.decode(reader, reader.uint32()));
          break;
        case 29:
          message.spells.push(reader.string());
          break;
        case 30:
          message.flavorTraits.push(reader.string());
          break;
        case 31:
          message.unusualFeatures = reader.string();
          break;
        case 32:
          message.cures = reader.string();
          break;
        case 33:
          message.documents = reader.string();
          break;
        case 34:
          message.notes = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Character, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Character | Character[]> | Iterable<Character | Character[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Character.encode(p).finish()];
        }
      } else {
        yield* [Character.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Character>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Character> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Character.decode(p)];
        }
      } else {
        yield* [Character.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Character {
    return {
      accountId: isSet(object.accountId) ? String(object.accountId) : "",
      characterName: isSet(object.characterName) ? String(object.characterName) : "",
      religions: isSet(object.religions) ? String(object.religions) : "",
      occupation: isSet(object.occupation) ? String(object.occupation) : "",
      specialty: isSet(object.specialty) ? String(object.specialty) : "",
      enhancement: isSet(object.enhancement) ? String(object.enhancement) : "",
      homeChapter: isSet(object.homeChapter) ? String(object.homeChapter) : "",
      publicStory: isSet(object.publicStory) ? String(object.publicStory) : "",
      privateStory: isSet(object.privateStory) ? String(object.privateStory) : "",
      homeland: isSet(object.homeland) ? String(object.homeland) : "",
      startingMoonstone: isSet(object.startingMoonstone) ? Number(object.startingMoonstone) : 0,
      skillTokens: isSet(object.skillTokens) ? Number(object.skillTokens) : 0,
      courage: isSet(object.courage) ? Number(object.courage) : 0,
      dexterity: isSet(object.dexterity) ? Number(object.dexterity) : 0,
      empathy: isSet(object.empathy) ? Number(object.empathy) : 0,
      passion: isSet(object.passion) ? Number(object.passion) : 0,
      prowess: isSet(object.prowess) ? Number(object.prowess) : 0,
      wisdom: isSet(object.wisdom) ? Number(object.wisdom) : 0,
      skills: Array.isArray(object?.skills) ? object.skills.map((e: any) => CharacterSkill.fromJSON(e)) : [],
      advantages: Array.isArray(object?.advantages)
        ? object.advantages.map((e: any) => CharacterVantage.fromJSON(e))
        : [],
      disadvantages: Array.isArray(object?.disadvantages)
        ? object.disadvantages.map((e: any) => CharacterVantage.fromJSON(e))
        : [],
      spells: Array.isArray(object?.spells) ? object.spells.map((e: any) => String(e)) : [],
      flavorTraits: Array.isArray(object?.flavorTraits) ? object.flavorTraits.map((e: any) => String(e)) : [],
      unusualFeatures: isSet(object.unusualFeatures) ? String(object.unusualFeatures) : undefined,
      cures: isSet(object.cures) ? String(object.cures) : undefined,
      documents: isSet(object.documents) ? String(object.documents) : undefined,
      notes: isSet(object.notes) ? String(object.notes) : undefined,
    };
  },

  toJSON(message: Character): unknown {
    const obj: any = {};
    message.accountId !== undefined && (obj.accountId = message.accountId);
    message.characterName !== undefined && (obj.characterName = message.characterName);
    message.religions !== undefined && (obj.religions = message.religions);
    message.occupation !== undefined && (obj.occupation = message.occupation);
    message.specialty !== undefined && (obj.specialty = message.specialty);
    message.enhancement !== undefined && (obj.enhancement = message.enhancement);
    message.homeChapter !== undefined && (obj.homeChapter = message.homeChapter);
    message.publicStory !== undefined && (obj.publicStory = message.publicStory);
    message.privateStory !== undefined && (obj.privateStory = message.privateStory);
    message.homeland !== undefined && (obj.homeland = message.homeland);
    message.startingMoonstone !== undefined && (obj.startingMoonstone = Math.round(message.startingMoonstone));
    message.skillTokens !== undefined && (obj.skillTokens = Math.round(message.skillTokens));
    message.courage !== undefined && (obj.courage = Math.round(message.courage));
    message.dexterity !== undefined && (obj.dexterity = Math.round(message.dexterity));
    message.empathy !== undefined && (obj.empathy = Math.round(message.empathy));
    message.passion !== undefined && (obj.passion = Math.round(message.passion));
    message.prowess !== undefined && (obj.prowess = Math.round(message.prowess));
    message.wisdom !== undefined && (obj.wisdom = Math.round(message.wisdom));
    if (message.skills) {
      obj.skills = message.skills.map((e) => e ? CharacterSkill.toJSON(e) : undefined);
    } else {
      obj.skills = [];
    }
    if (message.advantages) {
      obj.advantages = message.advantages.map((e) => e ? CharacterVantage.toJSON(e) : undefined);
    } else {
      obj.advantages = [];
    }
    if (message.disadvantages) {
      obj.disadvantages = message.disadvantages.map((e) => e ? CharacterVantage.toJSON(e) : undefined);
    } else {
      obj.disadvantages = [];
    }
    if (message.spells) {
      obj.spells = message.spells.map((e) => e);
    } else {
      obj.spells = [];
    }
    if (message.flavorTraits) {
      obj.flavorTraits = message.flavorTraits.map((e) => e);
    } else {
      obj.flavorTraits = [];
    }
    message.unusualFeatures !== undefined && (obj.unusualFeatures = message.unusualFeatures);
    message.cures !== undefined && (obj.cures = message.cures);
    message.documents !== undefined && (obj.documents = message.documents);
    message.notes !== undefined && (obj.notes = message.notes);
    return obj;
  },

  create<I extends Exact<DeepPartial<Character>, I>>(base?: I): Character {
    return Character.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Character>, I>>(object: I): Character {
    const message = createBaseCharacter();
    message.accountId = object.accountId ?? "";
    message.characterName = object.characterName ?? "";
    message.religions = object.religions ?? "";
    message.occupation = object.occupation ?? "";
    message.specialty = object.specialty ?? "";
    message.enhancement = object.enhancement ?? "";
    message.homeChapter = object.homeChapter ?? "";
    message.publicStory = object.publicStory ?? "";
    message.privateStory = object.privateStory ?? "";
    message.homeland = object.homeland ?? "";
    message.startingMoonstone = object.startingMoonstone ?? 0;
    message.skillTokens = object.skillTokens ?? 0;
    message.courage = object.courage ?? 0;
    message.dexterity = object.dexterity ?? 0;
    message.empathy = object.empathy ?? 0;
    message.passion = object.passion ?? 0;
    message.prowess = object.prowess ?? 0;
    message.wisdom = object.wisdom ?? 0;
    message.skills = object.skills?.map((e) => CharacterSkill.fromPartial(e)) || [];
    message.advantages = object.advantages?.map((e) => CharacterVantage.fromPartial(e)) || [];
    message.disadvantages = object.disadvantages?.map((e) => CharacterVantage.fromPartial(e)) || [];
    message.spells = object.spells?.map((e) => e) || [];
    message.flavorTraits = object.flavorTraits?.map((e) => e) || [];
    message.unusualFeatures = object.unusualFeatures ?? undefined;
    message.cures = object.cures ?? undefined;
    message.documents = object.documents ?? undefined;
    message.notes = object.notes ?? undefined;
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
