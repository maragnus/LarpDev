import * as jspb from 'google-protobuf'



export class CharacterSkill extends jspb.Message {
  getName(): string;
  setName(value: string): CharacterSkill;

  getRank(): number;
  setRank(value: number): CharacterSkill;

  getType(): SkillPurchase;
  setType(value: SkillPurchase): CharacterSkill;

  getPurchases(): number;
  setPurchases(value: number): CharacterSkill;
  hasPurchases(): boolean;
  clearPurchases(): CharacterSkill;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CharacterSkill.AsObject;
  static toObject(includeInstance: boolean, msg: CharacterSkill): CharacterSkill.AsObject;
  static serializeBinaryToWriter(message: CharacterSkill, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CharacterSkill;
  static deserializeBinaryFromReader(message: CharacterSkill, reader: jspb.BinaryReader): CharacterSkill;
}

export namespace CharacterSkill {
  export type AsObject = {
    name: string,
    rank: number,
    type: SkillPurchase,
    purchases?: number,
  }

  export enum PurchasesCase { 
    _PURCHASES_NOT_SET = 0,
    PURCHASES = 4,
  }
}

export class CharacterVantage extends jspb.Message {
  getName(): string;
  setName(value: string): CharacterVantage;

  getRank(): number;
  setRank(value: number): CharacterVantage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CharacterVantage.AsObject;
  static toObject(includeInstance: boolean, msg: CharacterVantage): CharacterVantage.AsObject;
  static serializeBinaryToWriter(message: CharacterVantage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CharacterVantage;
  static deserializeBinaryFromReader(message: CharacterVantage, reader: jspb.BinaryReader): CharacterVantage;
}

export namespace CharacterVantage {
  export type AsObject = {
    name: string,
    rank: number,
  }
}

export class Character extends jspb.Message {
  getAccountId(): string;
  setAccountId(value: string): Character;

  getCharactername(): string;
  setCharactername(value: string): Character;

  getReligions(): string;
  setReligions(value: string): Character;

  getOccupation(): string;
  setOccupation(value: string): Character;

  getSpecialty(): string;
  setSpecialty(value: string): Character;

  getEnhancement(): string;
  setEnhancement(value: string): Character;

  getHomechapter(): string;
  setHomechapter(value: string): Character;

  getPublicstory(): string;
  setPublicstory(value: string): Character;

  getPrivatestory(): string;
  setPrivatestory(value: string): Character;

  getHomeland(): string;
  setHomeland(value: string): Character;

  getStartingmoonstone(): number;
  setStartingmoonstone(value: number): Character;

  getSkilltokens(): number;
  setSkilltokens(value: number): Character;

  getCourage(): number;
  setCourage(value: number): Character;

  getDexterity(): number;
  setDexterity(value: number): Character;

  getEmpathy(): number;
  setEmpathy(value: number): Character;

  getPassion(): number;
  setPassion(value: number): Character;

  getProwess(): number;
  setProwess(value: number): Character;

  getWisdom(): number;
  setWisdom(value: number): Character;

  getSkillsList(): Array<CharacterSkill>;
  setSkillsList(value: Array<CharacterSkill>): Character;
  clearSkillsList(): Character;
  addSkills(value?: CharacterSkill, index?: number): CharacterSkill;

  getAdvantagesList(): Array<CharacterVantage>;
  setAdvantagesList(value: Array<CharacterVantage>): Character;
  clearAdvantagesList(): Character;
  addAdvantages(value?: CharacterVantage, index?: number): CharacterVantage;

  getDisadvantagesList(): Array<CharacterVantage>;
  setDisadvantagesList(value: Array<CharacterVantage>): Character;
  clearDisadvantagesList(): Character;
  addDisadvantages(value?: CharacterVantage, index?: number): CharacterVantage;

  getSpellsList(): Array<string>;
  setSpellsList(value: Array<string>): Character;
  clearSpellsList(): Character;
  addSpells(value: string, index?: number): Character;

  getFlavortraitsList(): Array<string>;
  setFlavortraitsList(value: Array<string>): Character;
  clearFlavortraitsList(): Character;
  addFlavortraits(value: string, index?: number): Character;

  getUnusualfeatures(): string;
  setUnusualfeatures(value: string): Character;
  hasUnusualfeatures(): boolean;
  clearUnusualfeatures(): Character;

  getCures(): string;
  setCures(value: string): Character;
  hasCures(): boolean;
  clearCures(): Character;

  getDocuments(): string;
  setDocuments(value: string): Character;
  hasDocuments(): boolean;
  clearDocuments(): Character;

  getNotes(): string;
  setNotes(value: string): Character;
  hasNotes(): boolean;
  clearNotes(): Character;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Character.AsObject;
  static toObject(includeInstance: boolean, msg: Character): Character.AsObject;
  static serializeBinaryToWriter(message: Character, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Character;
  static deserializeBinaryFromReader(message: Character, reader: jspb.BinaryReader): Character;
}

export namespace Character {
  export type AsObject = {
    accountId: string,
    charactername: string,
    religions: string,
    occupation: string,
    specialty: string,
    enhancement: string,
    homechapter: string,
    publicstory: string,
    privatestory: string,
    homeland: string,
    startingmoonstone: number,
    skilltokens: number,
    courage: number,
    dexterity: number,
    empathy: number,
    passion: number,
    prowess: number,
    wisdom: number,
    skillsList: Array<CharacterSkill.AsObject>,
    advantagesList: Array<CharacterVantage.AsObject>,
    disadvantagesList: Array<CharacterVantage.AsObject>,
    spellsList: Array<string>,
    flavortraitsList: Array<string>,
    unusualfeatures?: string,
    cures?: string,
    documents?: string,
    notes?: string,
  }

  export enum UnusualfeaturesCase { 
    _UNUSUALFEATURES_NOT_SET = 0,
    UNUSUALFEATURES = 31,
  }

  export enum CuresCase { 
    _CURES_NOT_SET = 0,
    CURES = 32,
  }

  export enum DocumentsCase { 
    _DOCUMENTS_NOT_SET = 0,
    DOCUMENTS = 33,
  }

  export enum NotesCase { 
    _NOTES_NOT_SET = 0,
    NOTES = 34,
  }
}

export enum SkillPurchase { 
  SKILL_PURCHASE_FREE = 0,
  SKILL_PURCHASE_OCCUPATION = 1,
  SKILL_PURCHASE_PURCHASE = 2,
  SKILL_PURCHASE_BESTOWED = 3,
}
