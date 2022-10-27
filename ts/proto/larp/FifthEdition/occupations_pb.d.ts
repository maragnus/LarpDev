import * as jspb from 'google-protobuf'



export class SkillChoice extends jspb.Message {
  getCount(): number;
  setCount(value: number): SkillChoice;

  getSkillsList(): Array<string>;
  setSkillsList(value: Array<string>): SkillChoice;
  clearSkillsList(): SkillChoice;
  addSkills(value: string, index?: number): SkillChoice;

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
    skillsList: Array<string>,
  }
}

export class Occupation extends jspb.Message {
  getName(): string;
  setName(value: string): Occupation;

  getSpecialtiesList(): Array<string>;
  setSpecialtiesList(value: Array<string>): Occupation;
  clearSpecialtiesList(): Occupation;
  addSpecialties(value: string, index?: number): Occupation;

  getType(): OccupationType;
  setType(value: OccupationType): Occupation;

  getSkills(): SkillChoice | undefined;
  setSkills(value?: SkillChoice): Occupation;
  hasSkills(): boolean;
  clearSkills(): Occupation;

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
    type: OccupationType,
    skills?: SkillChoice.AsObject,
    duty?: string,
    livery?: string,
  }

  export enum DutyCase { 
    _DUTY_NOT_SET = 0,
    DUTY = 5,
  }

  export enum LiveryCase { 
    _LIVERY_NOT_SET = 0,
    LIVERY = 6,
  }
}

export enum OccupationType { 
  OCCUPATION_TYPE_BASIC = 0,
  OCCUPATION_TYPE_YOUTH = 1,
  OCCUPATION_TYPE_ADVANCED = 2,
  OCCUPATION_TYPE_PLOT = 3,
  OCCUPATION_TYPE_ENHANCEMENT = 4,
}
