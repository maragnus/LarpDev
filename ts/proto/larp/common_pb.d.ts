import * as jspb from 'google-protobuf'



export class Game extends jspb.Message {
  getGameId(): number;
  setGameId(value: number): Game;

  getTitle(): string;
  setTitle(value: string): Game;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Game.AsObject;
  static toObject(includeInstance: boolean, msg: Game): Game.AsObject;
  static serializeBinaryToWriter(message: Game, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Game;
  static deserializeBinaryFromReader(message: Game, reader: jspb.BinaryReader): Game;
}

export namespace Game {
  export type AsObject = {
    gameId: number,
    title: string,
  }
}

