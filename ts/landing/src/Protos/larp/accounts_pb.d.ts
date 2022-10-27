import * as jspb from 'google-protobuf'



export class Account extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): Account;

  getName(): string;
  setName(value: string): Account;

  getLocation(): string;
  setLocation(value: string): Account;

  getEmailsList(): Array<AccountEmail>;
  setEmailsList(value: Array<AccountEmail>): Account;
  clearEmailsList(): Account;
  addEmails(value?: AccountEmail, index?: number): AccountEmail;

  getPhone(): string;
  setPhone(value: string): Account;

  getIsSuperAdmin(): boolean;
  setIsSuperAdmin(value: boolean): Account;

  getNotes(): string;
  setNotes(value: string): Account;

  getCreated(): string;
  setCreated(value: string): Account;

  getCharactersList(): Array<AccountCharacterSummary>;
  setCharactersList(value: Array<AccountCharacterSummary>): Account;
  clearCharactersList(): Account;
  addCharacters(value?: AccountCharacterSummary, index?: number): AccountCharacterSummary;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Account.AsObject;
  static toObject(includeInstance: boolean, msg: Account): Account.AsObject;
  static serializeBinaryToWriter(message: Account, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Account;
  static deserializeBinaryFromReader(message: Account, reader: jspb.BinaryReader): Account;
}

export namespace Account {
  export type AsObject = {
    accountId: string,
    name: string,
    location: string,
    emailsList: Array<AccountEmail.AsObject>,
    phone: string,
    isSuperAdmin: boolean,
    notes: string,
    created: string,
    charactersList: Array<AccountCharacterSummary.AsObject>,
  }
}

export class AccountEmail extends jspb.Message {
  getEmail(): string;
  setEmail(value: string): AccountEmail;

  getIsVerified(): boolean;
  setIsVerified(value: boolean): AccountEmail;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountEmail.AsObject;
  static toObject(includeInstance: boolean, msg: AccountEmail): AccountEmail.AsObject;
  static serializeBinaryToWriter(message: AccountEmail, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountEmail;
  static deserializeBinaryFromReader(message: AccountEmail, reader: jspb.BinaryReader): AccountEmail;
}

export namespace AccountEmail {
  export type AsObject = {
    email: string,
    isVerified: boolean,
  }
}

export class AccountCharacterSummary extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): AccountCharacterSummary;

  getGameId(): string;
  setGameId(value: string): AccountCharacterSummary;

  getAccountName(): string;
  setAccountName(value: string): AccountCharacterSummary;

  getCharacterId(): string;
  setCharacterId(value: string): AccountCharacterSummary;

  getCharacterName(): string;
  setCharacterName(value: string): AccountCharacterSummary;

  getHomeChapter(): string;
  setHomeChapter(value: string): AccountCharacterSummary;

  getSpecialty(): string;
  setSpecialty(value: string): AccountCharacterSummary;

  getLevel(): number;
  setLevel(value: number): AccountCharacterSummary;

  getIsLive(): boolean;
  setIsLive(value: boolean): AccountCharacterSummary;

  getIsReview(): boolean;
  setIsReview(value: boolean): AccountCharacterSummary;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountCharacterSummary.AsObject;
  static toObject(includeInstance: boolean, msg: AccountCharacterSummary): AccountCharacterSummary.AsObject;
  static serializeBinaryToWriter(message: AccountCharacterSummary, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountCharacterSummary;
  static deserializeBinaryFromReader(message: AccountCharacterSummary, reader: jspb.BinaryReader): AccountCharacterSummary;
}

export namespace AccountCharacterSummary {
  export type AsObject = {
    accountId: string,
    gameId: string,
    accountName: string,
    characterId: string,
    characterName: string,
    homeChapter: string,
    specialty: string,
    level: number,
    isLive: boolean,
    isReview: boolean,
  }
}

export class AccountAdmin extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): AccountAdmin;

  getGameId(): string;
  setGameId(value: string): AccountAdmin;

  getRank(): AdminRank;
  setRank(value: AdminRank): AccountAdmin;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AccountAdmin.AsObject;
  static toObject(includeInstance: boolean, msg: AccountAdmin): AccountAdmin.AsObject;
  static serializeBinaryToWriter(message: AccountAdmin, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AccountAdmin;
  static deserializeBinaryFromReader(message: AccountAdmin, reader: jspb.BinaryReader): AccountAdmin;
}

export namespace AccountAdmin {
  export type AsObject = {
    accountId: string,
    gameId: string,
    rank: AdminRank,
  }
}

export enum AdminRank { 
  ADMIN_RANK_NOT_ADMIN = 0,
  ADMIN_RANK_ADMIN = 1,
}
