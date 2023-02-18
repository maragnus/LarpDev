/* eslint-disable */
import * as _m0 from "protobufjs/minimal";

export const protobufPackage = "larp";

export const AdminRank = { ADMIN_RANK_NOT_ADMIN: 0, ADMIN_RANK_ADMIN: 1, UNRECOGNIZED: -1 } as const;

export type AdminRank = typeof AdminRank[keyof typeof AdminRank];

export function adminRankFromJSON(object: any): AdminRank {
  switch (object) {
    case 0:
    case "ADMIN_RANK_NOT_ADMIN":
      return AdminRank.ADMIN_RANK_NOT_ADMIN;
    case 1:
    case "ADMIN_RANK_ADMIN":
      return AdminRank.ADMIN_RANK_ADMIN;
    case -1:
    case "UNRECOGNIZED":
    default:
      return AdminRank.UNRECOGNIZED;
  }
}

export function adminRankToJSON(object: AdminRank): string {
  switch (object) {
    case AdminRank.ADMIN_RANK_NOT_ADMIN:
      return "ADMIN_RANK_NOT_ADMIN";
    case AdminRank.ADMIN_RANK_ADMIN:
      return "ADMIN_RANK_ADMIN";
    case AdminRank.UNRECOGNIZED:
    default:
      return "UNRECOGNIZED";
  }
}

export interface Account {
  accountId: string;
  name?: string | undefined;
  location?: string | undefined;
  emails: AccountEmail[];
  phone?: string | undefined;
  notes?: string | undefined;
  isSuperAdmin: boolean;
  created: string;
  characters: AccountCharacterSummary[];
}

export interface AccountEmail {
  email: string;
  isVerified: boolean;
  isPreferred: boolean;
}

export interface AccountCharacterSummary {
  gameId: string;
  accountId: string;
  accountName: string;
  characterId: string;
  characterName: string;
  homeChapter?: string | undefined;
  specialty?: string | undefined;
  level: number;
}

export interface AccountAdmin {
  accountId: string;
  gameId: string;
  rank: AdminRank;
}

function createBaseAccount(): Account {
  return {
    accountId: "",
    name: undefined,
    location: undefined,
    emails: [],
    phone: undefined,
    notes: undefined,
    isSuperAdmin: false,
    created: "",
    characters: [],
  };
}

export const Account = {
  encode(message: Account, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.accountId !== "") {
      writer.uint32(10).string(message.accountId);
    }
    if (message.name !== undefined) {
      writer.uint32(18).string(message.name);
    }
    if (message.location !== undefined) {
      writer.uint32(26).string(message.location);
    }
    for (const v of message.emails) {
      AccountEmail.encode(v!, writer.uint32(34).fork()).ldelim();
    }
    if (message.phone !== undefined) {
      writer.uint32(42).string(message.phone);
    }
    if (message.notes !== undefined) {
      writer.uint32(50).string(message.notes);
    }
    if (message.isSuperAdmin === true) {
      writer.uint32(56).bool(message.isSuperAdmin);
    }
    if (message.created !== "") {
      writer.uint32(66).string(message.created);
    }
    for (const v of message.characters) {
      AccountCharacterSummary.encode(v!, writer.uint32(74).fork()).ldelim();
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): Account {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccount();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.accountId = reader.string();
          break;
        case 2:
          message.name = reader.string();
          break;
        case 3:
          message.location = reader.string();
          break;
        case 4:
          message.emails.push(AccountEmail.decode(reader, reader.uint32()));
          break;
        case 5:
          message.phone = reader.string();
          break;
        case 6:
          message.notes = reader.string();
          break;
        case 7:
          message.isSuperAdmin = reader.bool();
          break;
        case 8:
          message.created = reader.string();
          break;
        case 9:
          message.characters.push(AccountCharacterSummary.decode(reader, reader.uint32()));
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<Account, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<Account | Account[]> | Iterable<Account | Account[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Account.encode(p).finish()];
        }
      } else {
        yield* [Account.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, Account>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<Account> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [Account.decode(p)];
        }
      } else {
        yield* [Account.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): Account {
    return {
      accountId: isSet(object.accountId) ? String(object.accountId) : "",
      name: isSet(object.name) ? String(object.name) : undefined,
      location: isSet(object.location) ? String(object.location) : undefined,
      emails: Array.isArray(object?.emails) ? object.emails.map((e: any) => AccountEmail.fromJSON(e)) : [],
      phone: isSet(object.phone) ? String(object.phone) : undefined,
      notes: isSet(object.notes) ? String(object.notes) : undefined,
      isSuperAdmin: isSet(object.isSuperAdmin) ? Boolean(object.isSuperAdmin) : false,
      created: isSet(object.created) ? String(object.created) : "",
      characters: Array.isArray(object?.characters)
        ? object.characters.map((e: any) => AccountCharacterSummary.fromJSON(e))
        : [],
    };
  },

  toJSON(message: Account): unknown {
    const obj: any = {};
    message.accountId !== undefined && (obj.accountId = message.accountId);
    message.name !== undefined && (obj.name = message.name);
    message.location !== undefined && (obj.location = message.location);
    if (message.emails) {
      obj.emails = message.emails.map((e) => e ? AccountEmail.toJSON(e) : undefined);
    } else {
      obj.emails = [];
    }
    message.phone !== undefined && (obj.phone = message.phone);
    message.notes !== undefined && (obj.notes = message.notes);
    message.isSuperAdmin !== undefined && (obj.isSuperAdmin = message.isSuperAdmin);
    message.created !== undefined && (obj.created = message.created);
    if (message.characters) {
      obj.characters = message.characters.map((e) => e ? AccountCharacterSummary.toJSON(e) : undefined);
    } else {
      obj.characters = [];
    }
    return obj;
  },

  create<I extends Exact<DeepPartial<Account>, I>>(base?: I): Account {
    return Account.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<Account>, I>>(object: I): Account {
    const message = createBaseAccount();
    message.accountId = object.accountId ?? "";
    message.name = object.name ?? undefined;
    message.location = object.location ?? undefined;
    message.emails = object.emails?.map((e) => AccountEmail.fromPartial(e)) || [];
    message.phone = object.phone ?? undefined;
    message.notes = object.notes ?? undefined;
    message.isSuperAdmin = object.isSuperAdmin ?? false;
    message.created = object.created ?? "";
    message.characters = object.characters?.map((e) => AccountCharacterSummary.fromPartial(e)) || [];
    return message;
  },
};

function createBaseAccountEmail(): AccountEmail {
  return { email: "", isVerified: false, isPreferred: false };
}

export const AccountEmail = {
  encode(message: AccountEmail, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.email !== "") {
      writer.uint32(10).string(message.email);
    }
    if (message.isVerified === true) {
      writer.uint32(16).bool(message.isVerified);
    }
    if (message.isPreferred === true) {
      writer.uint32(24).bool(message.isPreferred);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): AccountEmail {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccountEmail();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.email = reader.string();
          break;
        case 2:
          message.isVerified = reader.bool();
          break;
        case 3:
          message.isPreferred = reader.bool();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<AccountEmail, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<AccountEmail | AccountEmail[]> | Iterable<AccountEmail | AccountEmail[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountEmail.encode(p).finish()];
        }
      } else {
        yield* [AccountEmail.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, AccountEmail>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<AccountEmail> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountEmail.decode(p)];
        }
      } else {
        yield* [AccountEmail.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): AccountEmail {
    return {
      email: isSet(object.email) ? String(object.email) : "",
      isVerified: isSet(object.isVerified) ? Boolean(object.isVerified) : false,
      isPreferred: isSet(object.isPreferred) ? Boolean(object.isPreferred) : false,
    };
  },

  toJSON(message: AccountEmail): unknown {
    const obj: any = {};
    message.email !== undefined && (obj.email = message.email);
    message.isVerified !== undefined && (obj.isVerified = message.isVerified);
    message.isPreferred !== undefined && (obj.isPreferred = message.isPreferred);
    return obj;
  },

  create<I extends Exact<DeepPartial<AccountEmail>, I>>(base?: I): AccountEmail {
    return AccountEmail.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<AccountEmail>, I>>(object: I): AccountEmail {
    const message = createBaseAccountEmail();
    message.email = object.email ?? "";
    message.isVerified = object.isVerified ?? false;
    message.isPreferred = object.isPreferred ?? false;
    return message;
  },
};

function createBaseAccountCharacterSummary(): AccountCharacterSummary {
  return {
    gameId: "",
    accountId: "",
    accountName: "",
    characterId: "",
    characterName: "",
    homeChapter: undefined,
    specialty: undefined,
    level: 0,
  };
}

export const AccountCharacterSummary = {
  encode(message: AccountCharacterSummary, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.gameId !== "") {
      writer.uint32(10).string(message.gameId);
    }
    if (message.accountId !== "") {
      writer.uint32(18).string(message.accountId);
    }
    if (message.accountName !== "") {
      writer.uint32(26).string(message.accountName);
    }
    if (message.characterId !== "") {
      writer.uint32(34).string(message.characterId);
    }
    if (message.characterName !== "") {
      writer.uint32(42).string(message.characterName);
    }
    if (message.homeChapter !== undefined) {
      writer.uint32(50).string(message.homeChapter);
    }
    if (message.specialty !== undefined) {
      writer.uint32(58).string(message.specialty);
    }
    if (message.level !== 0) {
      writer.uint32(64).int32(message.level);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): AccountCharacterSummary {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccountCharacterSummary();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.gameId = reader.string();
          break;
        case 2:
          message.accountId = reader.string();
          break;
        case 3:
          message.accountName = reader.string();
          break;
        case 4:
          message.characterId = reader.string();
          break;
        case 5:
          message.characterName = reader.string();
          break;
        case 6:
          message.homeChapter = reader.string();
          break;
        case 7:
          message.specialty = reader.string();
          break;
        case 8:
          message.level = reader.int32();
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<AccountCharacterSummary, Uint8Array>
  async *encodeTransform(
    source:
      | AsyncIterable<AccountCharacterSummary | AccountCharacterSummary[]>
      | Iterable<AccountCharacterSummary | AccountCharacterSummary[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountCharacterSummary.encode(p).finish()];
        }
      } else {
        yield* [AccountCharacterSummary.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, AccountCharacterSummary>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<AccountCharacterSummary> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountCharacterSummary.decode(p)];
        }
      } else {
        yield* [AccountCharacterSummary.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): AccountCharacterSummary {
    return {
      gameId: isSet(object.gameId) ? String(object.gameId) : "",
      accountId: isSet(object.accountId) ? String(object.accountId) : "",
      accountName: isSet(object.accountName) ? String(object.accountName) : "",
      characterId: isSet(object.characterId) ? String(object.characterId) : "",
      characterName: isSet(object.characterName) ? String(object.characterName) : "",
      homeChapter: isSet(object.homeChapter) ? String(object.homeChapter) : undefined,
      specialty: isSet(object.specialty) ? String(object.specialty) : undefined,
      level: isSet(object.level) ? Number(object.level) : 0,
    };
  },

  toJSON(message: AccountCharacterSummary): unknown {
    const obj: any = {};
    message.gameId !== undefined && (obj.gameId = message.gameId);
    message.accountId !== undefined && (obj.accountId = message.accountId);
    message.accountName !== undefined && (obj.accountName = message.accountName);
    message.characterId !== undefined && (obj.characterId = message.characterId);
    message.characterName !== undefined && (obj.characterName = message.characterName);
    message.homeChapter !== undefined && (obj.homeChapter = message.homeChapter);
    message.specialty !== undefined && (obj.specialty = message.specialty);
    message.level !== undefined && (obj.level = Math.round(message.level));
    return obj;
  },

  create<I extends Exact<DeepPartial<AccountCharacterSummary>, I>>(base?: I): AccountCharacterSummary {
    return AccountCharacterSummary.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<AccountCharacterSummary>, I>>(object: I): AccountCharacterSummary {
    const message = createBaseAccountCharacterSummary();
    message.gameId = object.gameId ?? "";
    message.accountId = object.accountId ?? "";
    message.accountName = object.accountName ?? "";
    message.characterId = object.characterId ?? "";
    message.characterName = object.characterName ?? "";
    message.homeChapter = object.homeChapter ?? undefined;
    message.specialty = object.specialty ?? undefined;
    message.level = object.level ?? 0;
    return message;
  },
};

function createBaseAccountAdmin(): AccountAdmin {
  return { accountId: "", gameId: "", rank: 0 };
}

export const AccountAdmin = {
  encode(message: AccountAdmin, writer: _m0.Writer = _m0.Writer.create()): _m0.Writer {
    if (message.accountId !== "") {
      writer.uint32(10).string(message.accountId);
    }
    if (message.gameId !== "") {
      writer.uint32(18).string(message.gameId);
    }
    if (message.rank !== 0) {
      writer.uint32(24).int32(message.rank);
    }
    return writer;
  },

  decode(input: _m0.Reader | Uint8Array, length?: number): AccountAdmin {
    const reader = input instanceof _m0.Reader ? input : new _m0.Reader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAccountAdmin();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1:
          message.accountId = reader.string();
          break;
        case 2:
          message.gameId = reader.string();
          break;
        case 3:
          message.rank = reader.int32() as any;
          break;
        default:
          reader.skipType(tag & 7);
          break;
      }
    }
    return message;
  },

  // encodeTransform encodes a source of message objects.
  // Transform<AccountAdmin, Uint8Array>
  async *encodeTransform(
    source: AsyncIterable<AccountAdmin | AccountAdmin[]> | Iterable<AccountAdmin | AccountAdmin[]>,
  ): AsyncIterable<Uint8Array> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountAdmin.encode(p).finish()];
        }
      } else {
        yield* [AccountAdmin.encode(pkt).finish()];
      }
    }
  },

  // decodeTransform decodes a source of encoded messages.
  // Transform<Uint8Array, AccountAdmin>
  async *decodeTransform(
    source: AsyncIterable<Uint8Array | Uint8Array[]> | Iterable<Uint8Array | Uint8Array[]>,
  ): AsyncIterable<AccountAdmin> {
    for await (const pkt of source) {
      if (Array.isArray(pkt)) {
        for (const p of pkt) {
          yield* [AccountAdmin.decode(p)];
        }
      } else {
        yield* [AccountAdmin.decode(pkt)];
      }
    }
  },

  fromJSON(object: any): AccountAdmin {
    return {
      accountId: isSet(object.accountId) ? String(object.accountId) : "",
      gameId: isSet(object.gameId) ? String(object.gameId) : "",
      rank: isSet(object.rank) ? adminRankFromJSON(object.rank) : 0,
    };
  },

  toJSON(message: AccountAdmin): unknown {
    const obj: any = {};
    message.accountId !== undefined && (obj.accountId = message.accountId);
    message.gameId !== undefined && (obj.gameId = message.gameId);
    message.rank !== undefined && (obj.rank = adminRankToJSON(message.rank));
    return obj;
  },

  create<I extends Exact<DeepPartial<AccountAdmin>, I>>(base?: I): AccountAdmin {
    return AccountAdmin.fromPartial(base ?? {});
  },

  fromPartial<I extends Exact<DeepPartial<AccountAdmin>, I>>(object: I): AccountAdmin {
    const message = createBaseAccountAdmin();
    message.accountId = object.accountId ?? "";
    message.gameId = object.gameId ?? "";
    message.rank = object.rank ?? 0;
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
