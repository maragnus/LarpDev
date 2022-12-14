// source: larp/mw5e/skills.proto
/**
 * @fileoverview
 * @enhanceable
 * @suppress {missingRequire} reports error on implicit type usages.
 * @suppress {messageConventions} JS Compiler reports an error if a variable or
 *     field starts with 'MSG_' and isn't a translatable message.
 * @public
 */
// GENERATED CODE -- DO NOT EDIT!
/* eslint-disable */
// @ts-nocheck

var jspb = require('google-protobuf');
var goog = jspb;
var global =
    (typeof globalThis !== 'undefined' && globalThis) ||
    (typeof window !== 'undefined' && window) ||
    (typeof global !== 'undefined' && global) ||
    (typeof self !== 'undefined' && self) ||
    (function () { return this; }).call(null) ||
    Function('return this')();

goog.exportSymbol('proto.larp.mw5e.SkillClass', null, global);
goog.exportSymbol('proto.larp.mw5e.SkillDefinition', null, global);
goog.exportSymbol('proto.larp.mw5e.SkillPurchasable', null, global);
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.larp.mw5e.SkillDefinition = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.larp.mw5e.SkillDefinition.repeatedFields_, null);
};
goog.inherits(proto.larp.mw5e.SkillDefinition, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.mw5e.SkillDefinition.displayName = 'proto.larp.mw5e.SkillDefinition';
}

/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.larp.mw5e.SkillDefinition.repeatedFields_ = [7];



if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.larp.mw5e.SkillDefinition.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.mw5e.SkillDefinition.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.mw5e.SkillDefinition} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.mw5e.SkillDefinition.toObject = function(includeInstance, msg) {
  var f, obj = {
    name: jspb.Message.getFieldWithDefault(msg, 1, ""),
    title: jspb.Message.getFieldWithDefault(msg, 2, ""),
    pb_class: jspb.Message.getFieldWithDefault(msg, 3, ""),
    purchasable: jspb.Message.getFieldWithDefault(msg, 4, ""),
    ranksPerPurchase: jspb.Message.getFieldWithDefault(msg, 5, 0),
    costPerPurchase: jspb.Message.getFieldWithDefault(msg, 6, 0),
    iterationsList: (f = jspb.Message.getRepeatedField(msg, 7)) == null ? undefined : f
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.larp.mw5e.SkillDefinition}
 */
proto.larp.mw5e.SkillDefinition.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.mw5e.SkillDefinition;
  return proto.larp.mw5e.SkillDefinition.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.mw5e.SkillDefinition} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.mw5e.SkillDefinition}
 */
proto.larp.mw5e.SkillDefinition.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setName(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setTitle(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setClass(value);
      break;
    case 4:
      var value = /** @type {string} */ (reader.readString());
      msg.setPurchasable(value);
      break;
    case 5:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setRanksPerPurchase(value);
      break;
    case 6:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setCostPerPurchase(value);
      break;
    case 7:
      var value = /** @type {string} */ (reader.readString());
      msg.addIterations(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.larp.mw5e.SkillDefinition.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.mw5e.SkillDefinition.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.mw5e.SkillDefinition} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.mw5e.SkillDefinition.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getName();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getTitle();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getClass();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
  f = message.getPurchasable();
  if (f.length > 0) {
    writer.writeString(
      4,
      f
    );
  }
  f = /** @type {number} */ (jspb.Message.getField(message, 5));
  if (f != null) {
    writer.writeInt32(
      5,
      f
    );
  }
  f = /** @type {number} */ (jspb.Message.getField(message, 6));
  if (f != null) {
    writer.writeInt32(
      6,
      f
    );
  }
  f = message.getIterationsList();
  if (f.length > 0) {
    writer.writeRepeatedString(
      7,
      f
    );
  }
};


/**
 * optional string name = 1;
 * @return {string}
 */
proto.larp.mw5e.SkillDefinition.prototype.getName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setName = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string title = 2;
 * @return {string}
 */
proto.larp.mw5e.SkillDefinition.prototype.getTitle = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setTitle = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional string class = 3;
 * @return {string}
 */
proto.larp.mw5e.SkillDefinition.prototype.getClass = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setClass = function(value) {
  return jspb.Message.setProto3StringField(this, 3, value);
};


/**
 * optional string purchasable = 4;
 * @return {string}
 */
proto.larp.mw5e.SkillDefinition.prototype.getPurchasable = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 4, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setPurchasable = function(value) {
  return jspb.Message.setProto3StringField(this, 4, value);
};


/**
 * optional int32 ranks_per_purchase = 5;
 * @return {number}
 */
proto.larp.mw5e.SkillDefinition.prototype.getRanksPerPurchase = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 5, 0));
};


/**
 * @param {number} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setRanksPerPurchase = function(value) {
  return jspb.Message.setField(this, 5, value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.clearRanksPerPurchase = function() {
  return jspb.Message.setField(this, 5, undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.mw5e.SkillDefinition.prototype.hasRanksPerPurchase = function() {
  return jspb.Message.getField(this, 5) != null;
};


/**
 * optional int32 cost_per_purchase = 6;
 * @return {number}
 */
proto.larp.mw5e.SkillDefinition.prototype.getCostPerPurchase = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 6, 0));
};


/**
 * @param {number} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setCostPerPurchase = function(value) {
  return jspb.Message.setField(this, 6, value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.clearCostPerPurchase = function() {
  return jspb.Message.setField(this, 6, undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.mw5e.SkillDefinition.prototype.hasCostPerPurchase = function() {
  return jspb.Message.getField(this, 6) != null;
};


/**
 * repeated string iterations = 7;
 * @return {!Array<string>}
 */
proto.larp.mw5e.SkillDefinition.prototype.getIterationsList = function() {
  return /** @type {!Array<string>} */ (jspb.Message.getRepeatedField(this, 7));
};


/**
 * @param {!Array<string>} value
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.setIterationsList = function(value) {
  return jspb.Message.setField(this, 7, value || []);
};


/**
 * @param {string} value
 * @param {number=} opt_index
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.addIterations = function(value, opt_index) {
  return jspb.Message.addToRepeatedField(this, 7, value, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.larp.mw5e.SkillDefinition} returns this
 */
proto.larp.mw5e.SkillDefinition.prototype.clearIterationsList = function() {
  return this.setIterationsList([]);
};


/**
 * @enum {number}
 */
proto.larp.mw5e.SkillClass = {
  SKILL_CLASS_UNAVAILABLE: 0,
  SKILL_CLASS_FREE: 1,
  SKILL_CLASS_MINOR: 2,
  SKILL_CLASS_STANDARD: 3,
  SKILL_CLASS_MAJOR: 4
};

/**
 * @enum {number}
 */
proto.larp.mw5e.SkillPurchasable = {
  SKILL_PURCHASABLE_UNAVAILABLE: 0,
  SKILL_PURCHASABLE_ONCE: 1,
  SKILL_PURCHASABLE_MULTIPLE: 2
};

goog.object.extend(exports, proto.larp.mw5e);
