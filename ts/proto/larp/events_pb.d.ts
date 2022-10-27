import * as jspb from 'google-protobuf'

import * as larp_accounts_pb from '../larp/accounts_pb';


export class Event extends jspb.Message {
  getEventId(): string;
  setEventId(value: string): Event;

  getGameId(): string;
  setGameId(value: string): Event;

  getTitle(): string;
  setTitle(value: string): Event;

  getLocation(): string;
  setLocation(value: string): Event;

  getDate(): string;
  setDate(value: string): Event;

  getEventType(): string;
  setEventType(value: string): Event;

  getRsvp(): boolean;
  setRsvp(value: boolean): Event;

  getHidden(): boolean;
  setHidden(value: boolean): Event;

  getAttendeesList(): Array<AccountAttendance>;
  setAttendeesList(value: Array<AccountAttendance>): Event;
  clearAttendeesList(): Event;
  addAttendees(value?: AccountAttendance, index?: number): AccountAttendance;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Event.AsObject;
  static toObject(includeInstance: boolean, msg: Event): Event.AsObject;
  static serializeBinaryToWriter(message: Event, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Event;
  static deserializeBinaryFromReader(message: Event, reader: jspb.BinaryReader): Event;
}

export namespace Event {
  export type AsObject = {
    eventId: string,
    gameId: string,
    title: string,
    location: string,
    date: string,
    eventType: string,
    rsvp: boolean,
    hidden: boolean,
    attendeesList: Array<AccountAttendance.AsObject>,
  }
}

export class AccountAttendance extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): AccountAttendance;

  getName(): string;
  setName(value: string): AccountAttendance;

  getMoonstone(): number;
  setMoonstone(value: number): AccountAttendance;

  getRsvp(): EventRsvp;
  setRsvp(value: EventRsvp): AccountAttendance;

  getCharactersList(): Array<larp_accounts_pb.AccountCharacterSummary>;
  setCharactersList(value: Array<larp_accounts_pb.AccountCharacterSummary>): AccountAttendance;
  clearCharactersList(): AccountAttendance;
  addCharacters(value?: larp_accounts_pb.AccountCharacterSummary, index?: number): larp_accounts_pb.AccountCharacterSummary;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountAttendance.AsObject;
  static toObject(includeInstance: boolean, msg: AccountAttendance): AccountAttendance.AsObject;
  static serializeBinaryToWriter(message: AccountAttendance, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountAttendance;
  static deserializeBinaryFromReader(message: AccountAttendance, reader: jspb.BinaryReader): AccountAttendance;
}

export namespace AccountAttendance {
  export type AsObject = {
    accountId: string,
    name: string,
    moonstone: number,
    rsvp: EventRsvp,
    charactersList: Array<larp_accounts_pb.AccountCharacterSummary.AsObject>,
  }
}

export enum EventRsvp { 
  UNANSWERED = 0,
  NO = 1,
  MAYBE = 2,
  YES = 3,
  CONFIRMED = 4,
  APPROVED = 5,
}
