import * as jspb from 'google-protobuf'



export class Character extends jspb.Message {
  getAccountId(): number;
  setAccountId(value: number): Character;

  getName(): string;
  setName(value: string): Character;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Character.AsObject;
  static toObject(includeInstance: boolean, msg: Character): Character.AsObject;
  static serializeBinaryToWriter(message: Character, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Character;
  static deserializeBinaryFromReader(message: Character, reader: jspb.BinaryReader): Character;
}

export namespace Character {
  export type AsObject = {
    accountId: number,
    name: string,
  }
}

