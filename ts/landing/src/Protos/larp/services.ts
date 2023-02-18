/* eslint-disable */
import * as _m0 from "protobufjs/minimal";
import { Account } from "./accounts";
import { Empty, StringRequest } from "./common";
import {
  Event,
  EventAttendanceType,
  eventAttendanceTypeFromJSON,
  eventAttendanceTypeToJSON,
  EventRsvp,
  eventRsvpFromJSON,
  eventRsvpToJSON,
} from "./events";

export const protobufPackage = "larp.services";

export interface AccountResponse {
  account: Account | undefined;
}

export interface EventListRequest {
  includePast: boolean;
  includeFuture: boolean;
  includeAttendance: boolean;
}

export interface EventListResponse {
  event: Event[];
}

export interface EventRequest {
  eventId: string;
}

export interface EventComponentRsvp {
  componentName: string;
  type: EventAttendanceType;
  characterId?: string | undefined;
}

export interface EventRsvpRequest {
  eventId: string;
  characterId?: string | undefined;
  rsvp: EventRsvp;
  components: EventComponentRsvp[];
}

export interface UpdateProfileRequest {
  name?: string | undefined;
  phone?: string | undefined;
  location?: string | undefined;
  notes?: string | undefined;
}

export interface AccountRequest {
  account: Account | undefined;
}

function createBaseAccountResponse(): AccountResponse {
  return { account: undefined };
}

export const AccountResponse = {
  encode(message: AccountResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.account !== undefined) {
      Account.encode(message.account, writer.uint32(10).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): AccountResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccountResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.account = Account.decode(reader, reader.uint32());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<AccountResponse, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<AccountResponse | AccountResponse[]> | Iterable<AccountResponse | AccountResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountResponse.encode(p).finish()];
        }
      } else {
        yield* [AccountResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, AccountResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<AccountResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountResponse.decode(p)];
        }
      } else {
        yield* [AccountResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): AccountResponse {
    return { account: isSet(object.account) ? Account.fromJSON(object.account) : undefined };
  },

  toJSON(message: AccountResponse): unknown {
    const obj: any = {};
    message.account !== undefined && (obj.account = message.account ? Account.toJSON(message.account) : undefined);
    return obj;
  },

  create<I extends Exact<DeepPartial<AccountResponse>, I>>(base?: I): AccountResponse {
    return AccountResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<AccountResponse>, I>>(object: I): AccountResponse {
    const message = createBaseAccountResponse();
    message.account = (object.account !== undefined && object.account !== null)
      ? Account.fromPartial(object.account)
      : undefined;
    return message;
  },
};

function createBaseEventListRequest(): EventListRequest {
  return { includePast: false, includeFuture: false, includeAttendance: false };
}

export const EventListRequest = {
  encode(message: EventListRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.includePast === true) {
      writer.uint32(8).bool(message.includePast);
    }
    if (message.includeFuture === true) {
      writer.uint32(16).bool(message.includeFuture);
    }
    if (message.includeAttendance === true) {
      writer.uint32(24).bool(message.includeAttendance);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventListRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventListRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.includePast = reader.bool();
          break;
        case 2:
          message.includeFuture = reader.bool();
          break;
        case 3:
          message.includeAttendance = reader.bool();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventListRequest, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventListRequest | EventListRequest[]> | Iterable<EventListRequest | EventListRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventListRequest.encode(p).finish()];
        }
      } else {
        yield* [EventListRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventListRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventListRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventListRequest.decode(p)];
        }
      } else {
        yield* [EventListRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventListRequest {
    return {
      includePast: isSet(object.includePast) ? Boolean(object.includePast) : false,
      includeFuture: isSet(object.includeFuture) ? Boolean(object.includeFuture) : false,
      includeAttendance: isSet(object.includeAttendance) ? Boolean(object.includeAttendance) : false,
    };
  },

  toJSON(message: EventListRequest): unknown {
    const obj: any = {};
    message.includePast !== undefined && (obj.includePast = message.includePast);
    message.includeFuture !== undefined && (obj.includeFuture = message.includeFuture);
    message.includeAttendance !== undefined && (obj.includeAttendance = message.includeAttendance);
    return obj;
  },

  create<I extends Exact<DeepPartial<EventListRequest>, I>>(base?: I): EventListRequest {
    return EventListRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventListRequest>, I>>(object: I): EventListRequest {
    const message = createBaseEventListRequest();
    message.includePast = object.includePast ?? false;
    message.includeFuture = object.includeFuture ?? false;
    message.includeAttendance = object.includeAttendance ?? false;
    return message;
  },
};

function createBaseEventListResponse(): EventListResponse {
  return { event: [] };
}

export const EventListResponse = {
  encode(message: EventListResponse, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    for (const v of message.event) {
      Event.encode(v!, writer.uint32(10).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventListResponse {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventListResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.event.push(Event.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventListResponse, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventListResponse | EventListResponse[]> | Iterable<EventListResponse | EventListResponse[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventListResponse.encode(p).finish()];
        }
      } else {
        yield* [EventListResponse.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventListResponse>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventListResponse> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventListResponse.decode(p)];
        }
      } else {
        yield* [EventListResponse.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventListResponse {
    return { event: Array.isArray(object?.event) ? object.event.map((e: any) => Event.fromJSON(e)) : [] };
  },

  toJSON(message: EventListResponse): unknown {
    const obj: any = {};
    if (message.event) {
      obj.event = message.event.map((e) => e ? Event.toJSON(e) : undefined);
    } else {
      obj.event = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<EventListResponse>, I>>(base?: I): EventListResponse {
    return EventListResponse.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventListResponse>, I>>(object: I): EventListResponse {
    const message = createBaseEventListResponse();
    message.event = object.event?.map((e) => Event.fromPartial(e)) || [];
    return message;
  },
};

function createBaseEventRequest(): EventRequest {
  return { eventId: "" };
}

export const EventRequest = {
  encode(message: EventRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.eventId !== "") {
      writer.uint32(10).string(message.eventId);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.eventId = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventRequest, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventRequest | EventRequest[]> | Iterable<EventRequest | EventRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventRequest.encode(p).finish()];
        }
      } else {
        yield* [EventRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventRequest.decode(p)];
        }
      } else {
        yield* [EventRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventRequest {
    return { eventId: isSet(object.eventId) ? String(object.eventId) : "" };
  },

  toJSON(message: EventRequest): unknown {
    const obj: any = {};
    message.eventId !== undefined && (obj.eventId = message.eventId);
    return obj;
  },

  create<I extends Exact<DeepPartial<EventRequest>, I>>(base?: I): EventRequest {
    return EventRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventRequest>, I>>(object: I): EventRequest {
    const message = createBaseEventRequest();
    message.eventId = object.eventId ?? "";
    return message;
  },
};

function createBaseEventComponentRsvp(): EventComponentRsvp {
  return { componentName: "", type: 0, characterId: undefined };
}

export const EventComponentRsvp = {
  encode(message: EventComponentRsvp, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.componentName !== "") {
      writer.uint32(10).string(message.componentName);
    }
    if (message.type !== 0) {
      writer.uint32(16).int32(message.type);
    }
    if (message.characterId !== undefined) {
      writer.uint32(26).string(message.characterId);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventComponentRsvp {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventComponentRsvp();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.componentName = reader.string();
          break;
        case 2:
          message.type = reader.int32() as any;
          break;
        case 3:
          message.characterId = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventComponentRsvp, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<EventComponentRsvp | EventComponentRsvp[]>
      | Iterable<EventComponentRsvp | EventComponentRsvp[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponentRsvp.encode(p).finish()];
        }
      } else {
        yield* [EventComponentRsvp.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventComponentRsvp>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventComponentRsvp> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventComponentRsvp.decode(p)];
        }
      } else {
        yield* [EventComponentRsvp.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventComponentRsvp {
    return {
      componentName: isSet(object.componentName) ? String(object.componentName) : "",
      type: isSet(object.type) ? eventAttendanceTypeFromJSON(object.type) : 0,
      characterId: isSet(object.characterId) ? String(object.characterId) : undefined,
    };
  },

  toJSON(message: EventComponentRsvp): unknown {
    const obj: any = {};
    message.componentName !== undefined && (obj.componentName = message.componentName);
    message.type !== undefined && (obj.type = eventAttendanceTypeToJSON(message.type));
    message.characterId !== undefined && (obj.characterId = message.characterId);
    return obj;
  },

  create<I extends Exact<DeepPartial<EventComponentRsvp>, I>>(base?: I): EventComponentRsvp {
    return EventComponentRsvp.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventComponentRsvp>, I>>(object: I): EventComponentRsvp {
    const message = createBaseEventComponentRsvp();
    message.componentName = object.componentName ?? "";
    message.type = object.type ?? 0;
    message.characterId = object.characterId ?? undefined;
    return message;
  },
};

function createBaseEventRsvpRequest(): EventRsvpRequest {
  return { eventId: "", characterId: undefined, rsvp: 0, components: [] };
}

export const EventRsvpRequest = {
  encode(message: EventRsvpRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.eventId !== "") {
      writer.uint32(10).string(message.eventId);
    }
    if (message.characterId !== undefined) {
      writer.uint32(18).string(message.characterId);
    }
    if (message.rsvp !== 0) {
      writer.uint32(24).int32(message.rsvp);
    }
    for (const v of message.components) {
      EventComponentRsvp.encode(v!, writer.uint32(34).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): EventRsvpRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseEventRsvpRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.eventId = reader.string();
          break;
        case 2:
          message.characterId = reader.string();
          break;
        case 3:
          message.rsvp = reader.int32() as any;
          break;
        case 4:
          message.components.push(EventComponentRsvp.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<EventRsvpRequest, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<EventRsvpRequest | EventRsvpRequest[]> | Iterable<EventRsvpRequest | EventRsvpRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventRsvpRequest.encode(p).finish()];
        }
      } else {
        yield* [EventRsvpRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, EventRsvpRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<EventRsvpRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [EventRsvpRequest.decode(p)];
        }
      } else {
        yield* [EventRsvpRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): EventRsvpRequest {
    return {
      eventId: isSet(object.eventId) ? String(object.eventId) : "",
      characterId: isSet(object.characterId) ? String(object.characterId) : undefined,
      rsvp: isSet(object.rsvp) ? eventRsvpFromJSON(object.rsvp) : 0,
      components: Array.isArray(object?.components)
        ? object.components.map((e: any) => EventComponentRsvp.fromJSON(e))
        : [],
    };
  },

  toJSON(message: EventRsvpRequest): unknown {
    const obj: any = {};
    message.eventId !== undefined && (obj.eventId = message.eventId);
    message.characterId !== undefined && (obj.characterId = message.characterId);
    message.rsvp !== undefined && (obj.rsvp = eventRsvpToJSON(message.rsvp));
    if (message.components) {
      obj.components = message.components.map((e) => e ? EventComponentRsvp.toJSON(e) : undefined);
    } else {
      obj.components = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<EventRsvpRequest>, I>>(base?: I): EventRsvpRequest {
    return EventRsvpRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<EventRsvpRequest>, I>>(object: I): EventRsvpRequest {
    const message = createBaseEventRsvpRequest();
    message.eventId = object.eventId ?? "";
    message.characterId = object.characterId ?? undefined;
    message.rsvp = object.rsvp ?? 0;
    message.components = object.components?.map((e) => EventComponentRsvp.fromPartial(e)) || [];
    return message;
  },
};

function createBaseUpdateProfileRequest(): UpdateProfileRequest {
  return { name: undefined, phone: undefined, location: undefined, notes: undefined };
}

export const UpdateProfileRequest = {
  encode(message: UpdateProfileRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.name !== undefined) {
      writer.uint32(10).string(message.name);
    }
    if (message.phone !== undefined) {
      writer.uint32(18).string(message.phone);
    }
    if (message.location !== undefined) {
      writer.uint32(26).string(message.location);
    }
    if (message.notes !== undefined) {
      writer.uint32(34).string(message.notes);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): UpdateProfileRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseUpdateProfileRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.name = reader.string();
          break;
        case 2:
          message.phone = reader.string();
          break;
        case 3:
          message.location = reader.string();
          break;
        case 4:
          message.notes = reader.string();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<UpdateProfileRequest, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<UpdateProfileRequest | UpdateProfileRequest[]>
      | Iterable<UpdateProfileRequest | UpdateProfileRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [UpdateProfileRequest.encode(p).finish()];
        }
      } else {
        yield* [UpdateProfileRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, UpdateProfileRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<UpdateProfileRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [UpdateProfileRequest.decode(p)];
        }
      } else {
        yield* [UpdateProfileRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): UpdateProfileRequest {
    return {
      name: isSet(object.name) ? String(object.name) : undefined,
      phone: isSet(object.phone) ? String(object.phone) : undefined,
      location: isSet(object.location) ? String(object.location) : undefined,
      notes: isSet(object.notes) ? String(object.notes) : undefined,
    };
  },

  toJSON(message: UpdateProfileRequest): unknown {
    const obj: any = {};
    message.name !== undefined && (obj.name = message.name);
    message.phone !== undefined && (obj.phone = message.phone);
    message.location !== undefined && (obj.location = message.location);
    message.notes !== undefined && (obj.notes = message.notes);
    return obj;
  },

  create<I extends Exact<DeepPartial<UpdateProfileRequest>, I>>(base?: I): UpdateProfileRequest {
    return UpdateProfileRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<UpdateProfileRequest>, I>>(object: I): UpdateProfileRequest {
    const message = createBaseUpdateProfileRequest();
    message.name = object.name ?? undefined;
    message.phone = object.phone ?? undefined;
    message.location = object.location ?? undefined;
    message.notes = object.notes ?? undefined;
    return message;
  },
};

function createBaseAccountRequest(): AccountRequest {
  return { account: undefined };
}

export const AccountRequest = {
  encode(message: AccountRequest, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.account !== undefined) {
      Account.encode(message.account, writer.uint32(10).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): AccountRequest {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccountRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.account = Account.decode(reader, reader.uint32());
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<AccountRequest, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<AccountRequest | AccountRequest[]> | Iterable<AccountRequest | AccountRequest[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountRequest.encode(p).finish()];
        }
      } else {
        yield* [AccountRequest.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, AccountRequest>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<AccountRequest> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountRequest.decode(p)];
        }
      } else {
        yield* [AccountRequest.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): AccountRequest {
    return { account: isSet(object.account) ? Account.fromJSON(object.account) : undefined };
  },

  toJSON(message: AccountRequest): unknown {
    const obj: any = {};
    message.account !== undefined && (obj.account = message.account ? Account.toJSON(message.account) : undefined);
    return obj;
  },

  create<I extends Exact<DeepPartial<AccountRequest>, I>>(base?: I): AccountRequest {
    return AccountRequest.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<AccountRequest>, I>>(object: I): AccountRequest {
    const message = createBaseAccountRequest();
    message.account = (object.account !== undefined && object.account !== null)
      ? Account.fromPartial(object.account)
      : undefined;
    return message;
  },
};

export interface LarpUser {
  getAccount(request: Empty): Promise<AccountResponse>;
  updateProfile(request: UpdateProfileRequest): Promise<AccountResponse>;
  addEmail(request: StringRequest): Promise<AccountResponse>;
  removeEmail(request: StringRequest): Promise<AccountResponse>;
  preferEmail(request: StringRequest): Promise<AccountResponse>;
  getEvents(request: EventListRequest): Promise<EventListResponse>;
  rsvpEvent(request: EventRsvpRequest): Promise<Event>;
  getEvent(request: EventRequest): Promise<Event>;
}

export class LarpUserClientImpl implements LarpUser {
  private readonly rpc: Rpc;
  private readonly service: string;
  constructor(rpc: Rpc, opts?: { service?: string }) {
    this.service = opts?.service || "larp.services.LarpUser";
    this.rpc = rpc;
    this.getAccount = this.getAccount.bind(this);
    this.updateProfile = this.updateProfile.bind(this);
    this.addEmail = this.addEmail.bind(this);
    this.removeEmail = this.removeEmail.bind(this);
    this.preferEmail = this.preferEmail.bind(this);
    this.getEvents = this.getEvents.bind(this);
    this.rsvpEvent = this.rsvpEvent.bind(this);
    this.getEvent = this.getEvent.bind(this);
  }
  getAccount(request: Empty): Promise<AccountResponse> {
    const data = Empty.encode(request).finish();
    const promise = this.rpc.request(this.service, "GetAccount", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }

  updateProfile(request: UpdateProfileRequest): Promise<AccountResponse> {
    const data = UpdateProfileRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "UpdateProfile", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }

  addEmail(request: StringRequest): Promise<AccountResponse> {
    const data = StringRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "AddEmail", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }

  removeEmail(request: StringRequest): Promise<AccountResponse> {
    const data = StringRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "RemoveEmail", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }

  preferEmail(request: StringRequest): Promise<AccountResponse> {
    const data = StringRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "PreferEmail", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }

  getEvents(request: EventListRequest): Promise<EventListResponse> {
    const data = EventListRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "GetEvents", data);
    return promise.then((data) => EventListResponse.decode(new _m0.Reader(data)));
  }

  rsvpEvent(request: EventRsvpRequest): Promise<Event> {
    const data = EventRsvpRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "RsvpEvent", data);
    return promise.then((data) => Event.decode(new _m0.Reader(data)));
  }

  getEvent(request: EventRequest): Promise<Event> {
    const data = EventRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "GetEvent", data);
    return promise.then((data) => Event.decode(new _m0.Reader(data)));
  }
}

export interface LarpAdmin {
  setAccount(request: AccountRequest): Promise<AccountResponse>;
}

export class LarpAdminClientImpl implements LarpAdmin {
  private readonly rpc: Rpc;
  private readonly service: string;
  constructor(rpc: Rpc, opts?: { service?: string }) {
    this.service = opts?.service || "larp.services.LarpAdmin";
    this.rpc = rpc;
    this.setAccount = this.setAccount.bind(this);
  }
  setAccount(request: AccountRequest): Promise<AccountResponse> {
    const data = AccountRequest.encode(request).finish();
    const promise = this.rpc.request(this.service, "SetAccount", data);
    return promise.then((data) => AccountResponse.decode(new _m0.Reader(data)));
  }
}

interface Rpc {
  request(service: string, method: string, data: Uint8Array): Promise<Uint8Array>;
}

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
