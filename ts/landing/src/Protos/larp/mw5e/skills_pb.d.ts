import * as jspb from 'google-protobuf'



export class SkillDefinition extends jspb.Message {
  getName(): string;
  setName(value: string): SkillDefinition;

  getTitle(): string;
  setTitle(value: string): SkillDefinition;

  getClass(): string;
  setClass(value: string): SkillDefinition;

  getPurchasable(): string;
  setPurchasable(value: string): SkillDefinition;

  getRanksperpurchase(): number;
  setRanksperpurchase(value: number): SkillDefinition;
  hasRanksperpurchase(): boolean;
  clearRanksperpurchase(): SkillDefinition;

  getCostperpurchase(): number;
  setCostperpurchase(value: number): SkillDefinition;
  hasCostperpurchase(): boolean;
  clearCostperpurchase(): SkillDefinition;

  getIterationsList(): Array<string>;
  setIterationsList(value: Array<string>): SkillDefinition;
  clearIterationsList(): SkillDefinition;
  addIterations(value: string, index?: number): SkillDefinition;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SkillDefinition.AsObject;
  static toObject(includeInstance: boolean, msg: SkillDefinition): SkillDefinition.AsObject;
  static serializeBinaryToWriter(message: SkillDefinition, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SkillDefinition;
  static deserializeBinaryFromReader(message: SkillDefinition, reader: jspb.BinaryReader): SkillDefinition;
}

export namespace SkillDefinition {
  export type AsObject = {
    name: string,
    title: string,
    pb_class: string,
    purchasable: string,
    ranksperpurchase?: number,
    costperpurchase?: number,
    iterationsList: Array<string>,
  }

  export enum RanksperpurchaseCase { 
    _RANKSPERPURCHASE_NOT_SET = 0,
    RANKSPERPURCHASE = 5,
  }

  export enum CostperpurchaseCase { 
    _COSTPERPURCHASE_NOT_SET = 0,
    COSTPERPURCHASE = 6,
  }
}

export enum SkillClass { 
  SKILL_CLASS_UNAVAILABLE = 0,
  SKILL_CLASS_FREE = 1,
  SKILL_CLASS_MINOR = 2,
  SKILL_CLASS_STANDARD = 3,
  SKILL_CLASS_MAJOR = 4,
}
export enum SkillPurchasable { 
  SKILL_PURCHASABLE_UNAVAILABLE = 0,
  SKILL_PURCHASABLE_ONCE = 1,
  SKILL_PURCHASABLE_MULTIPLE = 2,
}
