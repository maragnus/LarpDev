/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp";

export const EventRsvp = {
  EVENT_RSVP_UNANSWERED: 0,
  /** EVENT_RSVP_NO - Will not attend */
  EVENT_RSVP_NO: 1,
  /** EVENT_RSVP_MAYBE - Potentially attending */
  EVENT_RSVP_MAYBE: 2,
  /** EVENT_RSVP_YES - Intention to attend */
  EVENT_RSVP_YES: 3,
  /** EVENT_RSVP_CONFIRMED - User confirmed their attendance (post letter) */
  EVENT_RSVP_CONFIRMED: 4,
  /** EVENT_RSVP_APPROVED - Admin has approved user's attendance */
  EVENT_RSVP_APPROVED: 5,
  UNRECOGNIZED: -1,
} as const;

export type EventRsvp = typeof EventRsvp[keyof typeof EventRsvp];

export function eventRsvpFromJSON(object: any): EventRsvp {
  switch (object) {
    case 0:
    case "EVENT_RSVP_UNANSWERED":
      return EventRsvp.EVENT_RSVP_UNANSWERED;
    case 1:
    case "EVENT_RSVP_NO":
      return EventRsvp.EVENT_RSVP_NO;
    case 2:
    case "EVENT_RSVP_MAYBE":
      return EventRsvp.EVENT_RSVP_MAYBE;
    case 3:
    case "EVENT_RSVP_YES":
      return EventRsvp.EVENT_RSVP_YES;
    case 4:
    case "EVENT_RSVP_CONFIRMED":
      return EventRsvp.EVENT_RSVP_CONFIRMED;
    case 5:
    case "EVENT_RSVP_APPROVED":
      return EventRsvp.EVENT_RSVP_APPROVED;
    case -1:
    case "UNRECOGNIZED":
    default:
      return EventRsvp.UNRECOGNIZED;
  }
}

export function eventRsvpToJSON(object: EventRsvp): string {
  switch (object) {
    case EventRsvp.EVENT_RSVP_UNANSWERED:
      return "EVENT_RSVP_UNANSWERED";
    case EventRsvp.EVENT_RSVP_NO:
      return "EVENT_RSVP_NO";
    case EventRsvp.EVENT_RSVP_MAYBE:
      return "EVENT_RSVP_MAYBE";
    case EventRsvp.EVENT_RSVP_YES:
      return "EVENT_RSVP_YES";
    case EventRsvp.EVENT_RSVP_CONFIRMED:
      return "EVENT_RSVP_CONFIRMED";
    case EventRsvp.EVENT_RSVP_APPROVED:
      return "EVENT_RSVP_APPROVED";
    case EventRsvp.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export const EventAttendanceType = {
  EVENT_ATTENDANCE_TYPE_PLAYER: 0,
  EVENT_ATTENDANCE_TYPE_STAFF: 1,
  EVENT_ATTENDANCE_TYPE_MIXED: 2,
  UNRECOGNIZED: -1,
} as const;

export type EventAttendanceType = typeof EventAttendanceType[keyof typeof EventAttendanceType];

export function eventAttendanceTypeFromJSON(object: any): EventAttendanceType {
  switch (object) {
    case 0:
    case "EVENT_ATTENDANCE_TYPE_PLAYER":
      return EventAttendanceType.EVENT_ATTENDANCE_TYPE_PLAYER;
    case 1:
    case "EVENT_ATTENDANCE_TYPE_STAFF":
      return EventAttendanceType.EVENT_ATTENDANCE_TYPE_STAFF;
    case 2:
    case "EVENT_ATTENDANCE_TYPE_MIXED":
      return EventAttendanceType.EVENT_ATTENDANCE_TYPE_MIXED;
    case -1:
    case "UNRECOGNIZED":
    default:
      return EventAttendanceType.UNRECOGNIZED;
  }
}

export function eventAttendanceTypeToJSON(object: EventAttendanceType): string {
  switch (object) {
    case EventAttendanceType.EVENT_ATTENDANCE_TYPE_PLAYER:
      return "EVENT_ATTENDANCE_TYPE_PLAYER";
    case EventAttendanceType.EVENT_ATTENDANCE_TYPE_STAFF:
      return "EVENT_ATTENDANCE_TYPE_STAFF";
    case EventAttendanceType.EVENT_ATTENDANCE_TYPE_MIXED:
      return "EVENT_ATTENDANCE_TYPE_MIXED";
    case EventAttendanceType.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface EventComponent {
  name: string;
  date: string;
}

export interface EventComponentAttendance {
  type: EventAttendanceType;
  characterId?: string | undefined;
  characterName?: string | undefined;
}

export interface EventAttendance {
  accountId: string;
  accountName: string;
  moonstone: number;
  rsvp: EventRsvp;
  components: EventComponentAttendance[];
}

export interface Event {
  eventId: string;
  gameId: string;
  title: string;
  location: string;
  date: string;
  eventType: string;
  rsvp: boolean;
  hidden: boolean;
  components: EventComponent[];
  attendees: EventAttendance[];
}

function createBaseEventComponent(): EventComponent {
  return { name: "", date: "" };
}

export const EventComponent = {
  encode(message: EventComponent, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    if (message.date !== "") {
      writer.uint32(18).string(message.date);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventComponent {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventComponent();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.date = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventComponent, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventComponent | EventComponent[]> | Iterable<EventComponent | EventComponent[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponent.encode(p).finish()];
        }
      } else {
        yield* [EventComponent.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventComponent>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventComponent> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponent.decode(p)];
        }
      } else {
        yield* [EventComponent.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventComponent {
    return { name: isSet(object.name) ? String(object.name) : "", date: isSet(object.date) ? String(object.date) : "" };
  },

  toJSON(message: EventComponent): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.date !== undefined && (obj.date = message.date);
    return obj;
  },

  create<I extends Exact<DeepPartial<EventComponent>, I>>(base?: I): EventComponent {
    return EventComponent.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventComponent>, I>>(object: I): EventComponent {
    const message = createBaseEventComponent();
    message.name = object.name ?? "";
    message.date = object.date ?? "";
    return message;
  },
};

function createBaseEventComponentAttendance(): EventComponentAttendance {
  return { type: 0, characterId: undefined, characterName: undefined };
}

export const EventComponentAttendance = {
  encode(message: EventComponentAttendance, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.type !== 0) {
      writer.uint32(8).int32(message.type);
    }
    if (message.characterId !== undefined) {
      writer.uint32(18).string(message.characterId);
    }
    if (message.characterName !== undefined) {
      writer.uint32(26).string(message.characterName);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventComponentAttendance {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventComponentAttendance();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.type = reader.int32() as any;
          break;
        case 2:
          message.characterId = reader.string();
          break;
        case 3:
          message.characterName = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventComponentAttendance, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<EventComponentAttendance | EventComponentAttendance[]>
      | Iterable<EventComponentAttendance | EventComponentAttendance[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponentAttendance.encode(p).finish()];
        }
      } else {
        yield* [EventComponentAttendance.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventComponentAttendance>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventComponentAttendance> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponentAttendance.decode(p)];
        }
      } else {
        yield* [EventComponentAttendance.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventComponentAttendance {
    return {
      type: isSet(object.type) ? eventAttendanceTypeFromJSON(object.type) : 0,
      characterId: isSet(object.characterId) ? String(object.characterId) : undefined,
      characterName: isSet(object.characterName) ? String(object.characterName) : undefined,
    };
  },

  toJSON(message: EventComponentAttendance): unknown {
    const obj: any = {};
    message.type !== undefined && (obj.type = eventAttendanceTypeToJSON(message.type));
    message.characterId !== undefined && (obj.characterId = message.characterId);
    message.characterName !== undefined && (obj.characterName = message.characterName);
    return obj;
  },

  create<I extends Exact<DeepPartial<EventComponentAttendance>, I>>(base?: I): EventComponentAttendance {
    return EventComponentAttendance.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventComponentAttendance>, I>>(object: I): EventComponentAttendance {
    const message = createBaseEventComponentAttendance();
    message.type = object.type ?? 0;
    message.characterId = object.characterId ?? undefined;
    message.characterName = object.characterName ?? undefined;
    return message;
  },
};

function createBaseEventAttendance(): EventAttendance {
  return { accountId: "", accountName: "", moonstone: 0, rsvp: 0, components: [] };
}

export const EventAttendance = {
  encode(message: EventAttendance, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.accountId !== "") {
      writer.uint32(10).string(message.accountId);
    }
    if (message.accountName !== "") {
      writer.uint32(18).string(message.accountName);
    }
    if (message.moonstone !== 0) {
      writer.uint32(40).int32(message.moonstone);
    }
    if (message.rsvp !== 0) {
      writer.uint32(48).int32(message.rsvp);
    }
    for (const v of message.components) {
      EventComponentAttendance.encode(v!, writer.uint32(58).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventAttendance {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventAttendance();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.accountId = reader.string();
          break;
        case 2:
          message.accountName = reader.string();
          break;
        case 5:
          message.moonstone = reader.int32();
          break;
        case 6:
          message.rsvp = reader.int32() as any;
          break;
        case 7:
          message.components.push(EventComponentAttendance.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventAttendance, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventAttendance | EventAttendance[]> | Iterable<EventAttendance | EventAttendance[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventAttendance.encode(p).finish()];
        }
      } else {
        yield* [EventAttendance.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventAttendance>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventAttendance> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventAttendance.decode(p)];
        }
      } else {
        yield* [EventAttendance.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventAttendance {
    return {
      accountId: isSet(object.accountId) ? String(object.accountId) : "",
      accountName: isSet(object.accountName) ? String(object.accountName) : "",
      moonstone: isSet(object.moonstone) ? Number(object.moonstone) : 0,
      rsvp: isSet(object.rsvp) ? eventRsvpFromJSON(object.rsvp) : 0,
      components: Array.isArray(object?.components)
        ? object.components.map((e: any) => EventComponentAttendance.fromJSON(e))
        : [],
    };
  },

  toJSON(message: EventAttendance): unknown {
    const obj: any = {};
    message.accountId !== undefined && (obj.accountId = message.accountId);
    message.accountName !== undefined && (obj.accountName = message.accountName);
    message.moonstone !== undefined && (obj.moonstone = Math.round(message.moonstone));
    message.rsvp !== undefined && (obj.rsvp = eventRsvpToJSON(message.rsvp));
    if (message.components) {
      obj.components = message.components.map((e) => e ? EventComponentAttendance.toJSON(e) : undefined);
    } else {
      obj.components = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<EventAttendance>, I>>(base?: I): EventAttendance {
    return EventAttendance.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventAttendance>, I>>(object: I): EventAttendance {
    const message = createBaseEventAttendance();
    message.accountId = object.accountId ?? "";
    message.accountName = object.accountName ?? "";
    message.moonstone = object.moonstone ?? 0;
    message.rsvp = object.rsvp ?? 0;
    message.components = object.components?.map((e) => EventComponentAttendance.fromPartial(e)) || [];
    return message;
  },
};

function createBaseEvent(): Event {
  return {
    eventId: "",
    gameId: "",
    title: "",
    location: "",
    date: "",
    eventType: "",
    rsvp: false,
    hidden: false,
    components: [],
    attendees: [],
  };
}

export const Event = {
  encode(message: Event, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.eventId !== "") {
      writer.uint32(10).string(message.eventId);
    }
    if (message.gameId !== "") {
      writer.uint32(18).string(message.gameId);
    }
    if (message.title !== "") {
      writer.uint32(26).string(message.title);
    }
    if (message.location !== "") {
      writer.uint32(34).string(message.location);
    }
    if (message.date !== "") {
      writer.uint32(42).string(message.date);
    }
    if (message.eventType !== "") {
      writer.uint32(50).string(message.eventType);
    }
    if (message.rsvp === true) {
      writer.uint32(56).bool(message.rsvp);
    }
    if (message.hidden === true) {
      writer.uint32(64).bool(message.hidden);
    }
    for (const v of message.components) {
      EventComponent.encode(v!, writer.uint32(74).fork()).ldelim();
    }
    for (const v of message.attendees) {
      EventAttendance.encode(v!, writer.uint32(82).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Event {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEvent();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.eventId = reader.string();
          break;
        case 2:
          message.gameId = reader.string();
          break;
        case 3:
          message.title = reader.string();
          break;
        case 4:
          message.location = reader.string();
          break;
        case 5:
          message.date = reader.string();
          break;
        case 6:
          message.eventType = reader.string();
          break;
        case 7:
          message.rsvp = reader.bool();
          break;
        case 8:
          message.hidden = reader.bool();
          break;
        case 9:
          message.components.push(EventComponent.decode(reader, reader.uint32()));
          break;
        case 10:
          message.attendees.push(EventAttendance.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Event, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Event | Event[]> | Iterable<Event | Event[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Event.encode(p).finish()];
        }
      } else {
        yield* [Event.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Event>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Event> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Event.decode(p)];
        }
      } else {
        yield* [Event.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Event {
    return {
      eventId: isSet(object.eventId) ? String(object.eventId) : "",
      gameId: isSet(object.gameId) ? String(object.gameId) : "",
      title: isSet(object.title) ? String(object.title) : "",
      location: isSet(object.location) ? String(object.location) : "",
      date: isSet(object.date) ? String(object.date) : "",
      eventType: isSet(object.eventType) ? String(object.eventType) : "",
      rsvp: isSet(object.rsvp) ? Boolean(object.rsvp) : false,
      hidden: isSet(object.hidden) ? Boolean(object.hidden) : false,
      components: Array.isArray(object?.components)
        ? object.components.map((e: any) => EventComponent.fromJSON(e))
        : [],
      attendees: Array.isArray(object?.attendees) ? object.attendees.map((e: any) => EventAttendance.fromJSON(e)) : [],
    };
  },

  toJSON(message: Event): unknown {
    const obj: any = {};
    message.eventId !== undefined && (obj.eventId = message.eventId);
    message.gameId !== undefined && (obj.gameId = message.gameId);
    message.title !== undefined && (obj.title = message.title);
    message.location !== undefined && (obj.location = message.location);
    message.date !== undefined && (obj.date = message.date);
    message.eventType !== undefined && (obj.eventType = message.eventType);
    message.rsvp !== undefined && (obj.rsvp = message.rsvp);
    message.hidden !== undefined && (obj.hidden = message.hidden);
    if (message.components) {
      obj.components = message.components.map((e) => e ? EventComponent.toJSON(e) : undefined);
    } else {
      obj.components = [];
    }
    if (message.attendees) {
      obj.attendees = message.attendees.map((e) => e ? EventAttendance.toJSON(e) : undefined);
    } else {
      obj.attendees = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<Event>, I>>(base?: I): Event {
    return Event.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Event>, I>>(object: I): Event {
    const message = createBaseEvent();
    message.eventId = object.eventId ?? "";
    message.gameId = object.gameId ?? "";
    message.title = object.title ?? "";
    message.location = object.location ?? "";
    message.date = object.date ?? "";
    message.eventType = object.eventType ?? "";
    message.rsvp = object.rsvp ?? false;
    message.hidden = object.hidden ?? false;
    message.components = object.components?.map((e) => EventComponent.fromPartial(e)) || [];
    message.attendees = object.attendees?.map((e) => EventAttendance.fromPartial(e)) || [];
    return message;
  },
};

type Builtin = Date | Function | Uint8Array | string | number | boolean | undefined;

export type DeepPartial<T> = T extends Builtin ? T
  : T extends Array<infer U> ? Array<DeepPartial<U>> : T extends ReadonlyArray<infer U> ? ReadonlyArray<DeepPartial<U>>
  : T extends {} ? { [K in keyof T]?: DeepPartial<T[K]> }
  : Partial<T>;

type KeysOfUnion<T> = T extends T ? keyof T : never;
export type Exact<P, I extends P> = P extends Builtin ? P
  : P & { [K in keyof P]: Exact<P[K], I[K]> } & { [K in Exclude<keyof I, KeysOfUnion<P>>]: never };

function isSet(value: any): boolean {
  return value !== null && value !== undefined;
}
