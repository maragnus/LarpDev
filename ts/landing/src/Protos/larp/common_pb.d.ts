import * as jspb from 'google-protobuf'



export class Game extends jspb.Message {
  getName(): string;
  setName(value: string): Game;

  getTitle(): string;
  setTitle(value: string): Game;

  getDescription(): string;
  setDescription(value: string): Game;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Game.AsObject;
  static toObject(includeInstance: boolean, msg: Game): Game.AsObject;
  static serializeBinaryToWriter(message: Game, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Game;
  static deserializeBinaryFromReader(message: Game, reader: jspb.BinaryReader): Game;
}

export namespace Game {
  export type AsObject = {
    name: string,
    title: string,
    description: string,
  }
}

export class State extends jspb.Message {
  getGamesList(): Array<Game>;
  setGamesList(value: Array<Game>): State;
  clearGamesList(): State;
  addGames(value?: Game, index?: number): Game;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): State.AsObject;
  static toObject(includeInstance: boolean, msg: State): State.AsObject;
  static serializeBinaryToWriter(message: State, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): State;
  static deserializeBinaryFromReader(message: State, reader: jspb.BinaryReader): State;
}

export namespace State {
  export type AsObject = {
    gamesList: Array<Game.AsObject>,
  }
}

