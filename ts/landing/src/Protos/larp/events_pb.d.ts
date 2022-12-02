import * as jspb from 'google-protobuf'



export class EventComponent extends jspb.Message {
  getName(): string;
  setName(value: string): EventComponent;

  getDate(): string;
  setDate(value: string): EventComponent;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventComponent.AsObject;
  static toObject(includeInstance: boolean, msg: EventComponent): EventComponent.AsObject;
  static serializeBinaryToWriter(message: EventComponent, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventComponent;
  static deserializeBinaryFromReader(message: EventComponent, reader: jspb.BinaryReader): EventComponent;
}

export namespace EventComponent {
  export type AsObject = {
    name: string,
    date: string,
  }
}

export class EventComponentAttendance extends jspb.Message {
  getType(): EventAttendanceType;
  setType(value: EventAttendanceType): EventComponentAttendance;

  getCharacterId(): string;
  setCharacterId(value: string): EventComponentAttendance;
  hasCharacterId(): boolean;
  clearCharacterId(): EventComponentAttendance;

  getCharacterName(): string;
  setCharacterName(value: string): EventComponentAttendance;
  hasCharacterName(): boolean;
  clearCharacterName(): EventComponentAttendance;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventComponentAttendance.AsObject;
  static toObject(includeInstance: boolean, msg: EventComponentAttendance): EventComponentAttendance.AsObject;
  static serializeBinaryToWriter(message: EventComponentAttendance, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventComponentAttendance;
  static deserializeBinaryFromReader(message: EventComponentAttendance, reader: jspb.BinaryReader): EventComponentAttendance;
}

export namespace EventComponentAttendance {
  export type AsObject = {
    type: EventAttendanceType,
    characterId?: string,
    characterName?: string,
  }

  export enum CharacterIdCase { 
    _CHARACTER_ID_NOT_SET = 0,
    CHARACTER_ID = 2,
  }

  export enum CharacterNameCase { 
    _CHARACTER_NAME_NOT_SET = 0,
    CHARACTER_NAME = 3,
  }
}

export class EventAttendance extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): EventAttendance;

  getAccountName(): string;
  setAccountName(value: string): EventAttendance;

  getMoonstone(): number;
  setMoonstone(value: number): EventAttendance;

  getRsvp(): EventRsvp;
  setRsvp(value: EventRsvp): EventAttendance;

  getComponentsList(): Array<EventComponentAttendance>;
  setComponentsList(value: Array<EventComponentAttendance>): EventAttendance;
  clearComponentsList(): EventAttendance;
  addComponents(value?: EventComponentAttendance, index?: number): EventComponentAttendance;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): EventAttendance.AsObject;
  static toObject(includeInstance: boolean, msg: EventAttendance): EventAttendance.AsObject;
  static serializeBinaryToWriter(message: EventAttendance, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): EventAttendance;
  static deserializeBinaryFromReader(message: EventAttendance, reader: jspb.BinaryReader): EventAttendance;
}

export namespace EventAttendance {
  export type AsObject = {
    accountId: string,
    accountName: string,
    moonstone: number,
    rsvp: EventRsvp,
    componentsList: Array<EventComponentAttendance.AsObject>,
  }
}

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

  getComponentsList(): Array<EventComponent>;
  setComponentsList(value: Array<EventComponent>): Event;
  clearComponentsList(): Event;
  addComponents(value?: EventComponent, index?: number): EventComponent;

  getAttendeesList(): Array<EventAttendance>;
  setAttendeesList(value: Array<EventAttendance>): Event;
  clearAttendeesList(): Event;
  addAttendees(value?: EventAttendance, index?: number): EventAttendance;

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
    componentsList: Array<EventComponent.AsObject>,
    attendeesList: Array<EventAttendance.AsObject>,
  }
}

export enum EventRsvp { 
  EVENT_RSVP_UNANSWERED = 0,
  EVENT_RSVP_NO = 1,
  EVENT_RSVP_MAYBE = 2,
  EVENT_RSVP_YES = 3,
  EVENT_RSVP_CONFIRMED = 4,
  EVENT_RSVP_APPROVED = 5,
}
export enum EventAttendanceType { 
  EVENT_ATTENDANCE_TYPE_PLAYER = 0,
  EVENT_ATTENDANCE_TYPE_STAFF = 1,
  EVENT_ATTENDANCE_TYPE_MIXED = 2,
}
