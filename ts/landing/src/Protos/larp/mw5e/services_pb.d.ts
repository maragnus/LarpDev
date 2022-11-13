import * as jspb from 'google-protobuf'

import * as larp_mw5e_fifthedition_pb from '../../larp/mw5e/fifthedition_pb';


export class UpdateCacheRequest extends jspb.Message {
  getLastUpdated(): string;
  setLastUpdated(value: string): UpdateCacheRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): UpdateCacheRequest.AsObject;
  static toObject(includeInstance: boolean, msg: UpdateCacheRequest): UpdateCacheRequest.AsObject;
  static serializeBinaryToWriter(message: UpdateCacheRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): UpdateCacheRequest;
  static deserializeBinaryFromReader(message: UpdateCacheRequest, reader: jspb.BinaryReader): UpdateCacheRequest;
}

export namespace UpdateCacheRequest {
  export type AsObject = {
    lastUpdated: string,
  }
}

export class GameStateResponse extends jspb.Message {
  getGameState(): larp_mw5e_fifthedition_pb.GameState | undefined;
  setGameState(value?: larp_mw5e_fifthedition_pb.GameState): GameStateResponse;
  hasGameState(): boolean;
  clearGameState(): GameStateResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GameStateResponse.AsObject;
  static toObject(includeInstance: boolean, msg: GameStateResponse): GameStateResponse.AsObject;
  static serializeBinaryToWriter(message: GameStateResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GameStateResponse;
  static deserializeBinaryFromReader(message: GameStateResponse, reader: jspb.BinaryReader): GameStateResponse;
}

export namespace GameStateResponse {
  export type AsObject = {
    gameState?: larp_mw5e_fifthedition_pb.GameState.AsObject,
  }

  export enum GameStateCase { 
    _GAME_STATE_NOT_SET = 0,
    GAME_STATE = 1,
  }
}

export class FindById extends jspb.Message {
  getId(): string;
  setId(value: string): FindById;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): FindById.AsObject;
  static toObject(includeInstance: boolean, msg: FindById): FindById.AsObject;
  static serializeBinaryToWriter(message: FindById, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): FindById;
  static deserializeBinaryFromReader(message: FindById, reader: jspb.BinaryReader): FindById;
}

export namespace FindById {
  export type AsObject = {
    id: string,
  }
}

