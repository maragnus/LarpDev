import * as jspb from 'google-protobuf'

import * as larp_accounts_pb from '../larp/accounts_pb';
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
    static toObject(includeInstance: boolean, msg: EventListRequest): EventListRequest.AsObject;

    static serializeBinaryToWriter(message: EventListRequest, writer: jspb.BinaryWriter): void;

    static deserializeBinary(bytes: Uint8Array): EventListRequest;

    static deserializeBinaryFromReader(message: EventListRequest, reader: jspb.BinaryReader): EventListRequest;

    getIncludePast(): boolean;

    setIncludePast(value: boolean): EventListRequest;

    getIncludeFuture(): boolean;

    setIncludeFuture(value: boolean): EventListRequest;

    getIncludeAttendance(): boolean;

    setIncludeAttendance(value: boolean): EventListRequest;

    serializeBinary(): Uint8Array;

    toObject(includeInstance?: boolean): EventListRequest.AsObject;
}

export namespace EventListRequest {
  export type AsObject = {
      includePast: boolean,
      includeFuture: boolean,
      includeAttendance: boolean,
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

export class UpdateProfileRequest extends jspb.Message {
    getName(): string;

    setName(value: string): UpdateProfileRequest;

    hasName(): boolean;

    clearName(): UpdateProfileRequest;

    getPhone(): string;

    setPhone(value: string): UpdateProfileRequest;

    hasPhone(): boolean;

    clearPhone(): UpdateProfileRequest;

    getLocation(): string;

    setLocation(value: string): UpdateProfileRequest;

    hasLocation(): boolean;

    clearLocation(): UpdateProfileRequest;

    getNotes(): string;

    setNotes(value: string): UpdateProfileRequest;

    hasNotes(): boolean;

    clearNotes(): UpdateProfileRequest;

    serializeBinary(): Uint8Array;

    toObject(includeInstance?: boolean): UpdateProfileRequest.AsObject;

    static toObject(includeInstance: boolean, msg: UpdateProfileRequest): UpdateProfileRequest.AsObject;

    static serializeBinaryToWriter(message: UpdateProfileRequest, writer: jspb.BinaryWriter): void;

    static deserializeBinary(bytes: Uint8Array): UpdateProfileRequest;

    static deserializeBinaryFromReader(message: UpdateProfileRequest, reader: jspb.BinaryReader): UpdateProfileRequest;
}

export namespace UpdateProfileRequest {
    export type AsObject = {
        name?: string,
        phone?: string,
        location?: string,
        notes?: string,
    }

    export enum NameCase {
        _NAME_NOT_SET = 0,
        NAME = 1,
    }

    export enum PhoneCase {
        _PHONE_NOT_SET = 0,
        PHONE = 2,
    }

    export enum LocationCase {
        _LOCATION_NOT_SET = 0,
        LOCATION = 3,
    }

    export enum NotesCase {
        _NOTES_NOT_SET = 0,
        NOTES = 4,
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

