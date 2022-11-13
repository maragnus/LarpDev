import * as jspb from 'google-protobuf'



export class Vantage extends jspb.Message {
  getName(): string;
  setName(value: string): Vantage;

  getTitle(): string;
  setTitle(value: string): Vantage;

  getRank(): number;
  setRank(value: number): Vantage;

  getPhysical(): boolean;
  setPhysical(value: boolean): Vantage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Vantage.AsObject;
  static toObject(includeInstance: boolean, msg: Vantage): Vantage.AsObject;
  static serializeBinaryToWriter(message: Vantage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Vantage;
  static deserializeBinaryFromReader(message: Vantage, reader: jspb.BinaryReader): Vantage;
}

export namespace Vantage {
  export type AsObject = {
    name: string,
    title: string,
    rank: number,
    physical: boolean,
  }
}

export class Religion extends jspb.Message {
  getName(): string;
  setName(value: string): Religion;

  getTitle(): string;
  setTitle(value: string): Religion;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Religion.AsObject;
  static toObject(includeInstance: boolean, msg: Religion): Religion.AsObject;
  static serializeBinaryToWriter(message: Religion, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Religion;
  static deserializeBinaryFromReader(message: Religion, reader: jspb.BinaryReader): Religion;
}

export namespace Religion {
  export type AsObject = {
    name: string,
    title: string,
  }
}

export class HomeChapter extends jspb.Message {
  getName(): string;
  setName(value: string): HomeChapter;

  getTitle(): string;
  setTitle(value: string): HomeChapter;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): HomeChapter.AsObject;
  static toObject(includeInstance: boolean, msg: HomeChapter): HomeChapter.AsObject;
  static serializeBinaryToWriter(message: HomeChapter, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): HomeChapter;
  static deserializeBinaryFromReader(message: HomeChapter, reader: jspb.BinaryReader): HomeChapter;
}

export namespace HomeChapter {
  export type AsObject = {
    name: string,
    title: string,
  }
}

export class Spell extends jspb.Message {
  getName(): string;
  setName(value: string): Spell;

  getType(): string;
  setType(value: string): Spell;

  getCategory(): string;
  setCategory(value: string): Spell;

  getMana(): number;
  setMana(value: number): Spell;

  getEffect(): string;
  setEffect(value: string): Spell;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Spell.AsObject;
  static toObject(includeInstance: boolean, msg: Spell): Spell.AsObject;
  static serializeBinaryToWriter(message: Spell, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Spell;
  static deserializeBinaryFromReader(message: Spell, reader: jspb.BinaryReader): Spell;
}

export namespace Spell {
  export type AsObject = {
    name: string,
    type: string,
    category: string,
    mana: number,
    effect: string,
  }
}

export enum SpellType { 
  SPELL_TYPE_BOLT = 0,
  SPELL_TYPE_GESTURE = 1,
  SPELL_TYPE_SPRAY = 2,
  SPELL_TYPE_VOICE = 3,
  SPELL_TYPE_STORM = 4,
  SPELL_TYPE_ROOM = 5,
  SPELL_TYPE_GESTURE_OR_VOICE = 6,
}
