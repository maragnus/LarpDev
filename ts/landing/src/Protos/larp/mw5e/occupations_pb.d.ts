import * as jspb from 'google-protobuf'



export class SkillChoice extends jspb.Message {
  getCount(): number;
  setCount(value: number): SkillChoice;

  getChoicesList(): Array<string>;
  setChoicesList(value: Array<string>): SkillChoice;
  clearChoicesList(): SkillChoice;
  addChoices(value: string, index?: number): SkillChoice;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SkillChoice.AsObject;
  static toObject(includeInstance: boolean, msg: SkillChoice): SkillChoice.AsObject;
  static serializeBinaryToWriter(message: SkillChoice, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SkillChoice;
  static deserializeBinaryFromReader(message: SkillChoice, reader: jspb.BinaryReader): SkillChoice;
}

export namespace SkillChoice {
  export type AsObject = {
    count: number,
    choicesList: Array<string>,
  }
}

export class Occupation extends jspb.Message {
  getName(): string;
  setName(value: string): Occupation;

  getSpecialtiesList(): Array<string>;
  setSpecialtiesList(value: Array<string>): Occupation;
  clearSpecialtiesList(): Occupation;
  addSpecialties(value: string, index?: number): Occupation;

  getType(): string;
  setType(value: string): Occupation;

  getSkillsList(): Array<string>;
  setSkillsList(value: Array<string>): Occupation;
  clearSkillsList(): Occupation;
  addSkills(value: string, index?: number): Occupation;

  getChoicesList(): Array<SkillChoice>;
  setChoicesList(value: Array<SkillChoice>): Occupation;
  clearChoicesList(): Occupation;
  addChoices(value?: SkillChoice, index?: number): SkillChoice;

  getDuty(): string;
  setDuty(value: string): Occupation;
  hasDuty(): boolean;
  clearDuty(): Occupation;

  getLivery(): string;
  setLivery(value: string): Occupation;
  hasLivery(): boolean;
  clearLivery(): Occupation;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): Occupation.AsObject;
  static toObject(includeInstance: boolean, msg: Occupation): Occupation.AsObject;
  static serializeBinaryToWriter(message: Occupation, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): Occupation;
  static deserializeBinaryFromReader(message: Occupation, reader: jspb.BinaryReader): Occupation;
}

export namespace Occupation {
  export type AsObject = {
    name: string,
    specialtiesList: Array<string>,
    type: string,
    skillsList: Array<string>,
    choicesList: Array<SkillChoice.AsObject>,
    duty?: string,
    livery?: string,
  }

  export enum DutyCase { 
    _DUTY_NOT_SET = 0,
    DUTY = 6,
  }

  export enum LiveryCase { 
    _LIVERY_NOT_SET = 0,
    LIVERY = 7,
  }
}

export enum OccupationType { 
  OCCUPATION_TYPE_BASIC = 0,
  OCCUPATION_TYPE_YOUTH = 1,
  OCCUPATION_TYPE_ADVANCED = 2,
  OCCUPATION_TYPE_PLOT = 3,
  OCCUPATION_TYPE_ENHANCEMENT = 4,
}
