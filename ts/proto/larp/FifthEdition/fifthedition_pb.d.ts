import * as jspb from 'google-protobuf'

import * as larp_FifthEdition_character_pb from '../../larp/FifthEdition/character_pb';
import * as larp_FifthEdition_gifts_pb from '../../larp/FifthEdition/gifts_pb';
import * as larp_FifthEdition_occupations_pb from '../../larp/FifthEdition/occupations_pb';
import * as larp_FifthEdition_other_pb from '../../larp/FifthEdition/other_pb';
import * as larp_FifthEdition_skills_pb from '../../larp/FifthEdition/skills_pb';


export class GameState extends jspb.Message {
  getGiftsList(): Array<larp_FifthEdition_gifts_pb.Gift>;
  setGiftsList(value: Array<larp_FifthEdition_gifts_pb.Gift>): GameState;
  clearGiftsList(): GameState;
  addGifts(value?: larp_FifthEdition_gifts_pb.Gift, index?: number): larp_FifthEdition_gifts_pb.Gift;

  getSkillsList(): Array<larp_FifthEdition_skills_pb.SkillDefinition>;
  setSkillsList(value: Array<larp_FifthEdition_skills_pb.SkillDefinition>): GameState;
  clearSkillsList(): GameState;
  addSkills(value?: larp_FifthEdition_skills_pb.SkillDefinition, index?: number): larp_FifthEdition_skills_pb.SkillDefinition;

  getOccupationsList(): Array<larp_FifthEdition_occupations_pb.Occupation>;
  setOccupationsList(value: Array<larp_FifthEdition_occupations_pb.Occupation>): GameState;
  clearOccupationsList(): GameState;
  addOccupations(value?: larp_FifthEdition_occupations_pb.Occupation, index?: number): larp_FifthEdition_occupations_pb.Occupation;

  getVantagesList(): Array<larp_FifthEdition_other_pb.Vantage>;
  setVantagesList(value: Array<larp_FifthEdition_other_pb.Vantage>): GameState;
  clearVantagesList(): GameState;
  addVantages(value?: larp_FifthEdition_other_pb.Vantage, index?: number): larp_FifthEdition_other_pb.Vantage;

  getReligionsList(): Array<larp_FifthEdition_other_pb.Religion>;
  setReligionsList(value: Array<larp_FifthEdition_other_pb.Religion>): GameState;
  clearReligionsList(): GameState;
  addReligions(value?: larp_FifthEdition_other_pb.Religion, index?: number): larp_FifthEdition_other_pb.Religion;

  getHomeChaptersList(): Array<larp_FifthEdition_other_pb.HomeChapter>;
  setHomeChaptersList(value: Array<larp_FifthEdition_other_pb.HomeChapter>): GameState;
  clearHomeChaptersList(): GameState;
  addHomeChapters(value?: larp_FifthEdition_other_pb.HomeChapter, index?: number): larp_FifthEdition_other_pb.HomeChapter;

  getSpellsList(): Array<larp_FifthEdition_other_pb.Spell>;
  setSpellsList(value: Array<larp_FifthEdition_other_pb.Spell>): GameState;
  clearSpellsList(): GameState;
  addSpells(value?: larp_FifthEdition_other_pb.Spell, index?: number): larp_FifthEdition_other_pb.Spell;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GameState.AsObject;
  static toObject(includeInstance: boolean, msg: GameState): GameState.AsObject;
  static serializeBinaryToWriter(message: GameState, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GameState;
  static deserializeBinaryFromReader(message: GameState, reader: jspb.BinaryReader): GameState;
}

export namespace GameState {
  export type AsObject = {
    giftsList: Array<larp_FifthEdition_gifts_pb.Gift.AsObject>,
    skillsList: Array<larp_FifthEdition_skills_pb.SkillDefinition.AsObject>,
    occupationsList: Array<larp_FifthEdition_occupations_pb.Occupation.AsObject>,
    vantagesList: Array<larp_FifthEdition_other_pb.Vantage.AsObject>,
    religionsList: Array<larp_FifthEdition_other_pb.Religion.AsObject>,
    homeChaptersList: Array<larp_FifthEdition_other_pb.HomeChapter.AsObject>,
    spellsList: Array<larp_FifthEdition_other_pb.Spell.AsObject>,
  }
}

