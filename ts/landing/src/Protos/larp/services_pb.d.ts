import * as jspb from 'google-protobuf'

import * as larp_accounts_pb from '../larp/accounts_pb';
import * as larp_common_pb from '../larp/common_pb';
import * as larp_events_pb from '../larp/events_pb';


export class AccountResponse extends jspb.Message {
  getAccount(): larp_accounts_pb.Account | undefined;
  setAccount(value?: larp_accounts_pb.Account): AccountResponse;
  hasAccount(): boolean;
  clearAccount(): AccountResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountResponse.AsObject;
  static toObject(includeInstance: boolean, msg: AccountResponse): AccountResponse.AsObject;
  static serializeBinaryToWriter(message: AccountResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountResponse;
  static deserializeBinaryFromReader(message: AccountResponse, reader: jspb.BinaryReader): AccountResponse;
}

export namespace AccountResponse {
  export type AsObject = {
    account?: larp_accounts_pb.Account.AsObject,
  }
}

export class EventListRequest extends jspb.Message {
  getIncludepast(): boolean;
  setIncludepast(value: boolean): EventListRequest;

  getIncludefuture(): boolean;
  setIncludefuture(value: boolean): EventListRequest;

  getIncludeattendance(): boolean;
  setIncludeattendance(value: boolean): EventListRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventListRequest.AsObject;
  static toObject(includeInstance: boolean, msg: EventListRequest): EventListRequest.AsObject;
  static serializeBinaryToWriter(message: EventListRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventListRequest;
  static deserializeBinaryFromReader(message: EventListRequest, reader: jspb.BinaryReader): EventListRequest;
}

export namespace EventListRequest {
  export type AsObject = {
    includepast: boolean,
    includefuture: boolean,
    includeattendance: boolean,
  }
}

export class EventListResponse extends jspb.Message {
  getEventList(): Array<larp_events_pb.Event>;
  setEventList(value: Array<larp_events_pb.Event>): EventListResponse;
  clearEventList(): EventListResponse;
  addEvent(value?: larp_events_pb.Event, index?: number): larp_events_pb.Event;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventListResponse.AsObject;
  static toObject(includeInstance: boolean, msg: EventListResponse): EventListResponse.AsObject;
  static serializeBinaryToWriter(message: EventListResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventListResponse;
  static deserializeBinaryFromReader(message: EventListResponse, reader: jspb.BinaryReader): EventListResponse;
}

export namespace EventListResponse {
  export type AsObject = {
    eventList: Array<larp_events_pb.Event.AsObject>,
  }
}

export class EventComponentRsvp extends jspb.Message {
  getComponentName(): string;
  setComponentName(value: string): EventComponentRsvp;

  getType(): larp_events_pb.EventAttendanceType;
  setType(value: larp_events_pb.EventAttendanceType): EventComponentRsvp;

  getCharacterId(): string;
  setCharacterId(value: string): EventComponentRsvp;
  hasCharacterId(): boolean;
  clearCharacterId(): EventComponentRsvp;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventComponentRsvp.AsObject;
  static toObject(includeInstance: boolean, msg: EventComponentRsvp): EventComponentRsvp.AsObject;
  static serializeBinaryToWriter(message: EventComponentRsvp, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventComponentRsvp;
  static deserializeBinaryFromReader(message: EventComponentRsvp, reader: jspb.BinaryReader): EventComponentRsvp;
}

export namespace EventComponentRsvp {
  export type AsObject = {
    componentName: string,
    type: larp_events_pb.EventAttendanceType,
    characterId?: string,
  }

  export enum CharacterIdCase { 
    _CHARACTER_ID_NOT_SET = 0,
    CHARACTER_ID = 3,
  }
}

export class EventRsvpRequest extends jspb.Message {
  getEventId(): string;
  setEventId(value: string): EventRsvpRequest;

  getCharacterId(): string;
  setCharacterId(value: string): EventRsvpRequest;
  hasCharacterId(): boolean;
  clearCharacterId(): EventRsvpRequest;

  getRsvp(): larp_events_pb.EventRsvp;
  setRsvp(value: larp_events_pb.EventRsvp): EventRsvpRequest;

  getComponentsList(): Array<EventComponentRsvp>;
  setComponentsList(value: Array<EventComponentRsvp>): EventRsvpRequest;
  clearComponentsList(): EventRsvpRequest;
  addComponents(value?: EventComponentRsvp, index?: number): EventComponentRsvp;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventRsvpRequest.AsObject;
  static toObject(includeInstance: boolean, msg: EventRsvpRequest): EventRsvpRequest.AsObject;
  static serializeBinaryToWriter(message: EventRsvpRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventRsvpRequest;
  static deserializeBinaryFromReader(message: EventRsvpRequest, reader: jspb.BinaryReader): EventRsvpRequest;
}

export namespace EventRsvpRequest {
  export type AsObject = {
    eventId: string,
    characterId?: string,
    rsvp: larp_events_pb.EventRsvp,
    componentsList: Array<EventComponentRsvp.AsObject>,
  }

  export enum CharacterIdCase { 
    _CHARACTER_ID_NOT_SET = 0,
    CHARACTER_ID = 2,
  }
}

export class AccountRequest extends jspb.Message {
  getAccount(): larp_accounts_pb.Account | undefined;
  setAccount(value?: larp_accounts_pb.Account): AccountRequest;
  hasAccount(): boolean;
  clearAccount(): AccountRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AccountRequest): AccountRequest.AsObject;
  static serializeBinaryToWriter(message: AccountRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountRequest;
  static deserializeBinaryFromReader(message: AccountRequest, reader: jspb.BinaryReader): AccountRequest;
}

export namespace AccountRequest {
  export type AsObject = {
    account?: larp_accounts_pb.Account.AsObject,
  }
}

