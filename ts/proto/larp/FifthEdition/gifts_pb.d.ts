import * as jspb from 'google-protobuf'



export class Gift extends jspb.Message {
  getName(): string;
  setName(value: string): Gift;

  getTitle(): string;
  setTitle(value: string): Gift;

  getPropertiesList(): Array<string>;
  setPropertiesList(value: Array<string>): Gift;
  clearPropertiesList(): Gift;
  addProperties(value: string, index?: number): Gift;

  getRanksList(): Array<GiftRank>;
  setRanksList(value: Array<GiftRank>): Gift;
  clearRanksList(): Gift;
  addRanks(value?: GiftRank, index?: number): GiftRank;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Gift.AsObject;
  static toObject(includeInstance: boolean, msg: Gift): Gift.AsObject;
  static serializeBinaryToWriter(message: Gift, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Gift;
  static deserializeBinaryFromReader(message: Gift, reader: jspb.BinaryReader): Gift;
}

export namespace Gift {
  export type AsObject = {
    name: string,
    title: string,
    propertiesList: Array<string>,
    ranksList: Array<GiftRank.AsObject>,
  }
}

export class GiftProperty extends jspb.Message {
  getName(): string;
  setName(value: string): GiftProperty;

  getTitle(): string;
  setTitle(value: string): GiftProperty;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GiftProperty.AsObject;
  static toObject(includeInstance: boolean, msg: GiftProperty): GiftProperty.AsObject;
  static serializeBinaryToWriter(message: GiftProperty, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GiftProperty;
  static deserializeBinaryFromReader(message: GiftProperty, reader: jspb.BinaryReader): GiftProperty;
}

export namespace GiftProperty {
  export type AsObject = {
    name: string,
    title: string,
  }
}

export class GiftPropertyValue extends jspb.Message {
  getName(): string;
  setName(value: string): GiftPropertyValue;

  getValue(): string;
  setValue(value: string): GiftPropertyValue;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GiftPropertyValue.AsObject;
  static toObject(includeInstance: boolean, msg: GiftPropertyValue): GiftPropertyValue.AsObject;
  static serializeBinaryToWriter(message: GiftPropertyValue, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GiftPropertyValue;
  static deserializeBinaryFromReader(message: GiftPropertyValue, reader: jspb.BinaryReader): GiftPropertyValue;
}

export namespace GiftPropertyValue {
  export type AsObject = {
    name: string,
    value: string,
  }
}

export class GiftRank extends jspb.Message {
  getRank(): number;
  setRank(value: number): GiftRank;

  getPropertiesList(): Array<string>;
  setPropertiesList(value: Array<string>): GiftRank;
  clearPropertiesList(): GiftRank;
  addProperties(value: string, index?: number): GiftRank;

  getAbilitiesList(): Array<Ability>;
  setAbilitiesList(value: Array<Ability>): GiftRank;
  clearAbilitiesList(): GiftRank;
  addAbilities(value?: Ability, index?: number): Ability;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GiftRank.AsObject;
  static toObject(includeInstance: boolean, msg: GiftRank): GiftRank.AsObject;
  static serializeBinaryToWriter(message: GiftRank, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GiftRank;
  static deserializeBinaryFromReader(message: GiftRank, reader: jspb.BinaryReader): GiftRank;
}

export namespace GiftRank {
  export type AsObject = {
    rank: number,
    propertiesList: Array<string>,
    abilitiesList: Array<Ability.AsObject>,
  }
}

export class Ability extends jspb.Message {
  getName(): string;
  setName(value: string): Ability;

  getRank(): number;
  setRank(value: number): Ability;

  getTitle(): string;
  setTitle(value: string): Ability;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Ability.AsObject;
  static toObject(includeInstance: boolean, msg: Ability): Ability.AsObject;
  static serializeBinaryToWriter(message: Ability, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Ability;
  static deserializeBinaryFromReader(message: Ability, reader: jspb.BinaryReader): Ability;
}

export namespace Ability {
  export type AsObject = {
    name: string,
    rank: number,
    title: string,
  }
}

