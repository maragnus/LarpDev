// source: larp/accounts.proto
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

goog.exportSymbol('proto.larp.Account', null, global);
goog.exportSymbol('proto.larp.AccountAdmin', null, global);
goog.exportSymbol('proto.larp.AccountCharacterSummary', null, global);
goog.exportSymbol('proto.larp.AccountEmail', null, global);
goog.exportSymbol('proto.larp.AdminRank', null, global);
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
proto.larp.Account = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.larp.Account.repeatedFields_, null);
};
goog.inherits(proto.larp.Account, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.Account.displayName = 'proto.larp.Account';
}
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
proto.larp.AccountEmail = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.AccountEmail, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.AccountEmail.displayName = 'proto.larp.AccountEmail';
}
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
proto.larp.AccountCharacterSummary = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.AccountCharacterSummary, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.AccountCharacterSummary.displayName = 'proto.larp.AccountCharacterSummary';
}
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
proto.larp.AccountAdmin = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.AccountAdmin, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.AccountAdmin.displayName = 'proto.larp.AccountAdmin';
}

/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.larp.Account.repeatedFields_ = [4,9];



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
proto.larp.Account.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.Account.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.Account} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.Account.toObject = function(includeInstance, msg) {
  var f, obj = {
    accountId: jspb.Message.getFieldWithDefault(msg, 1, ""),
    name: jspb.Message.getFieldWithDefault(msg, 2, ""),
    location: jspb.Message.getFieldWithDefault(msg, 3, ""),
    emailsList: jspb.Message.toObjectList(msg.getEmailsList(),
    proto.larp.AccountEmail.toObject, includeInstance),
    phone: jspb.Message.getFieldWithDefault(msg, 5, ""),
    isSuperAdmin: jspb.Message.getBooleanFieldWithDefault(msg, 6, false),
    notes: jspb.Message.getFieldWithDefault(msg, 7, ""),
    created: jspb.Message.getFieldWithDefault(msg, 8, ""),
    charactersList: jspb.Message.toObjectList(msg.getCharactersList(),
    proto.larp.AccountCharacterSummary.toObject, includeInstance)
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
 * @return {!proto.larp.Account}
 */
proto.larp.Account.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.Account;
  return proto.larp.Account.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.Account} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.Account}
 */
proto.larp.Account.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setAccountId(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setName(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setLocation(value);
      break;
    case 4:
      var value = new proto.larp.AccountEmail;
      reader.readMessage(value,proto.larp.AccountEmail.deserializeBinaryFromReader);
      msg.addEmails(value);
      break;
    case 5:
      var value = /** @type {string} */ (reader.readString());
      msg.setPhone(value);
      break;
    case 6:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIsSuperAdmin(value);
      break;
    case 7:
      var value = /** @type {string} */ (reader.readString());
      msg.setNotes(value);
      break;
    case 8:
      var value = /** @type {string} */ (reader.readString());
      msg.setCreated(value);
      break;
    case 9:
      var value = new proto.larp.AccountCharacterSummary;
      reader.readMessage(value,proto.larp.AccountCharacterSummary.deserializeBinaryFromReader);
      msg.addCharacters(value);
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
proto.larp.Account.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.Account.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.Account} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.Account.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAccountId();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getName();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getLocation();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
  f = message.getEmailsList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      4,
      f,
      proto.larp.AccountEmail.serializeBinaryToWriter
    );
  }
  f = message.getPhone();
  if (f.length > 0) {
    writer.writeString(
      5,
      f
    );
  }
  f = message.getIsSuperAdmin();
  if (f) {
    writer.writeBool(
      6,
      f
    );
  }
  f = message.getNotes();
  if (f.length > 0) {
    writer.writeString(
      7,
      f
    );
  }
  f = message.getCreated();
  if (f.length > 0) {
    writer.writeString(
      8,
      f
    );
  }
  f = message.getCharactersList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      9,
      f,
      proto.larp.AccountCharacterSummary.serializeBinaryToWriter
    );
  }
};


/**
 * optional string account_id = 1;
 * @return {string}
 */
proto.larp.Account.prototype.getAccountId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setAccountId = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string name = 2;
 * @return {string}
 */
proto.larp.Account.prototype.getName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setName = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional string location = 3;
 * @return {string}
 */
proto.larp.Account.prototype.getLocation = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setLocation = function(value) {
  return jspb.Message.setProto3StringField(this, 3, value);
};


/**
 * repeated AccountEmail emails = 4;
 * @return {!Array<!proto.larp.AccountEmail>}
 */
proto.larp.Account.prototype.getEmailsList = function() {
  return /** @type{!Array<!proto.larp.AccountEmail>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.larp.AccountEmail, 4));
};


/**
 * @param {!Array<!proto.larp.AccountEmail>} value
 * @return {!proto.larp.Account} returns this
*/
proto.larp.Account.prototype.setEmailsList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 4, value);
};


/**
 * @param {!proto.larp.AccountEmail=} opt_value
 * @param {number=} opt_index
 * @return {!proto.larp.AccountEmail}
 */
proto.larp.Account.prototype.addEmails = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 4, opt_value, proto.larp.AccountEmail, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.clearEmailsList = function() {
  return this.setEmailsList([]);
};


/**
 * optional string phone = 5;
 * @return {string}
 */
proto.larp.Account.prototype.getPhone = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 5, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setPhone = function(value) {
  return jspb.Message.setProto3StringField(this, 5, value);
};


/**
 * optional bool is_super_admin = 6;
 * @return {boolean}
 */
proto.larp.Account.prototype.getIsSuperAdmin = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 6, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setIsSuperAdmin = function(value) {
  return jspb.Message.setProto3BooleanField(this, 6, value);
};


/**
 * optional string notes = 7;
 * @return {string}
 */
proto.larp.Account.prototype.getNotes = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 7, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setNotes = function(value) {
  return jspb.Message.setProto3StringField(this, 7, value);
};


/**
 * optional string created = 8;
 * @return {string}
 */
proto.larp.Account.prototype.getCreated = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 8, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.setCreated = function(value) {
  return jspb.Message.setProto3StringField(this, 8, value);
};


/**
 * repeated AccountCharacterSummary characters = 9;
 * @return {!Array<!proto.larp.AccountCharacterSummary>}
 */
proto.larp.Account.prototype.getCharactersList = function() {
  return /** @type{!Array<!proto.larp.AccountCharacterSummary>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.larp.AccountCharacterSummary, 9));
};


/**
 * @param {!Array<!proto.larp.AccountCharacterSummary>} value
 * @return {!proto.larp.Account} returns this
*/
proto.larp.Account.prototype.setCharactersList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 9, value);
};


/**
 * @param {!proto.larp.AccountCharacterSummary=} opt_value
 * @param {number=} opt_index
 * @return {!proto.larp.AccountCharacterSummary}
 */
proto.larp.Account.prototype.addCharacters = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 9, opt_value, proto.larp.AccountCharacterSummary, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.larp.Account} returns this
 */
proto.larp.Account.prototype.clearCharactersList = function() {
  return this.setCharactersList([]);
};





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
proto.larp.AccountEmail.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.AccountEmail.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.AccountEmail} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountEmail.toObject = function(includeInstance, msg) {
  var f, obj = {
    email: jspb.Message.getFieldWithDefault(msg, 1, ""),
    isVerified: jspb.Message.getBooleanFieldWithDefault(msg, 2, false)
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
 * @return {!proto.larp.AccountEmail}
 */
proto.larp.AccountEmail.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.AccountEmail;
  return proto.larp.AccountEmail.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.AccountEmail} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.AccountEmail}
 */
proto.larp.AccountEmail.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setEmail(value);
      break;
    case 2:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIsVerified(value);
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
proto.larp.AccountEmail.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.AccountEmail.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.AccountEmail} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountEmail.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getEmail();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getIsVerified();
  if (f) {
    writer.writeBool(
      2,
      f
    );
  }
};


/**
 * optional string email = 1;
 * @return {string}
 */
proto.larp.AccountEmail.prototype.getEmail = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountEmail} returns this
 */
proto.larp.AccountEmail.prototype.setEmail = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional bool is_verified = 2;
 * @return {boolean}
 */
proto.larp.AccountEmail.prototype.getIsVerified = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 2, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.AccountEmail} returns this
 */
proto.larp.AccountEmail.prototype.setIsVerified = function(value) {
  return jspb.Message.setProto3BooleanField(this, 2, value);
};





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
proto.larp.AccountCharacterSummary.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.AccountCharacterSummary.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.AccountCharacterSummary} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountCharacterSummary.toObject = function(includeInstance, msg) {
  var f, obj = {
    accountId: jspb.Message.getFieldWithDefault(msg, 1, ""),
    gameId: jspb.Message.getFieldWithDefault(msg, 2, ""),
    accountName: jspb.Message.getFieldWithDefault(msg, 3, ""),
    characterId: jspb.Message.getFieldWithDefault(msg, 4, ""),
    characterName: jspb.Message.getFieldWithDefault(msg, 5, ""),
    homeChapter: jspb.Message.getFieldWithDefault(msg, 6, ""),
    specialty: jspb.Message.getFieldWithDefault(msg, 7, ""),
    level: jspb.Message.getFieldWithDefault(msg, 8, 0),
    isLive: jspb.Message.getBooleanFieldWithDefault(msg, 9, false),
    isReview: jspb.Message.getBooleanFieldWithDefault(msg, 10, false)
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
 * @return {!proto.larp.AccountCharacterSummary}
 */
proto.larp.AccountCharacterSummary.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.AccountCharacterSummary;
  return proto.larp.AccountCharacterSummary.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.AccountCharacterSummary} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.AccountCharacterSummary}
 */
proto.larp.AccountCharacterSummary.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setAccountId(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setGameId(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setAccountName(value);
      break;
    case 4:
      var value = /** @type {string} */ (reader.readString());
      msg.setCharacterId(value);
      break;
    case 5:
      var value = /** @type {string} */ (reader.readString());
      msg.setCharacterName(value);
      break;
    case 6:
      var value = /** @type {string} */ (reader.readString());
      msg.setHomeChapter(value);
      break;
    case 7:
      var value = /** @type {string} */ (reader.readString());
      msg.setSpecialty(value);
      break;
    case 8:
      var value = /** @type {number} */ (reader.readInt32());
      msg.setLevel(value);
      break;
    case 9:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIsLive(value);
      break;
    case 10:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIsReview(value);
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
proto.larp.AccountCharacterSummary.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.AccountCharacterSummary.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.AccountCharacterSummary} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountCharacterSummary.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAccountId();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getGameId();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getAccountName();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
  f = message.getCharacterId();
  if (f.length > 0) {
    writer.writeString(
      4,
      f
    );
  }
  f = message.getCharacterName();
  if (f.length > 0) {
    writer.writeString(
      5,
      f
    );
  }
  f = message.getHomeChapter();
  if (f.length > 0) {
    writer.writeString(
      6,
      f
    );
  }
  f = message.getSpecialty();
  if (f.length > 0) {
    writer.writeString(
      7,
      f
    );
  }
  f = message.getLevel();
  if (f !== 0) {
    writer.writeInt32(
      8,
      f
    );
  }
  f = message.getIsLive();
  if (f) {
    writer.writeBool(
      9,
      f
    );
  }
  f = message.getIsReview();
  if (f) {
    writer.writeBool(
      10,
      f
    );
  }
};


/**
 * optional string account_id = 1;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getAccountId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setAccountId = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string game_id = 2;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getGameId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setGameId = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional string account_name = 3;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getAccountName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setAccountName = function(value) {
  return jspb.Message.setProto3StringField(this, 3, value);
};


/**
 * optional string character_id = 4;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getCharacterId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 4, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setCharacterId = function(value) {
  return jspb.Message.setProto3StringField(this, 4, value);
};


/**
 * optional string character_name = 5;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getCharacterName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 5, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setCharacterName = function(value) {
  return jspb.Message.setProto3StringField(this, 5, value);
};


/**
 * optional string home_chapter = 6;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getHomeChapter = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 6, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setHomeChapter = function(value) {
  return jspb.Message.setProto3StringField(this, 6, value);
};


/**
 * optional string specialty = 7;
 * @return {string}
 */
proto.larp.AccountCharacterSummary.prototype.getSpecialty = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 7, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setSpecialty = function(value) {
  return jspb.Message.setProto3StringField(this, 7, value);
};


/**
 * optional int32 level = 8;
 * @return {number}
 */
proto.larp.AccountCharacterSummary.prototype.getLevel = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 8, 0));
};


/**
 * @param {number} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setLevel = function(value) {
  return jspb.Message.setProto3IntField(this, 8, value);
};


/**
 * optional bool is_live = 9;
 * @return {boolean}
 */
proto.larp.AccountCharacterSummary.prototype.getIsLive = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 9, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setIsLive = function(value) {
  return jspb.Message.setProto3BooleanField(this, 9, value);
};


/**
 * optional bool is_review = 10;
 * @return {boolean}
 */
proto.larp.AccountCharacterSummary.prototype.getIsReview = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 10, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.AccountCharacterSummary} returns this
 */
proto.larp.AccountCharacterSummary.prototype.setIsReview = function(value) {
  return jspb.Message.setProto3BooleanField(this, 10, value);
};





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
proto.larp.AccountAdmin.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.AccountAdmin.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.AccountAdmin} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountAdmin.toObject = function(includeInstance, msg) {
  var f, obj = {
    accountId: jspb.Message.getFieldWithDefault(msg, 1, ""),
    gameId: jspb.Message.getFieldWithDefault(msg, 2, ""),
    rank: jspb.Message.getFieldWithDefault(msg, 3, 0)
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
 * @return {!proto.larp.AccountAdmin}
 */
proto.larp.AccountAdmin.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.AccountAdmin;
  return proto.larp.AccountAdmin.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.AccountAdmin} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.AccountAdmin}
 */
proto.larp.AccountAdmin.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setAccountId(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setGameId(value);
      break;
    case 3:
      var value = /** @type {!proto.larp.AdminRank} */ (reader.readEnum());
      msg.setRank(value);
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
proto.larp.AccountAdmin.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.AccountAdmin.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.AccountAdmin} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.AccountAdmin.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAccountId();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getGameId();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getRank();
  if (f !== 0.0) {
    writer.writeEnum(
      3,
      f
    );
  }
};


/**
 * optional string account_id = 1;
 * @return {string}
 */
proto.larp.AccountAdmin.prototype.getAccountId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountAdmin} returns this
 */
proto.larp.AccountAdmin.prototype.setAccountId = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string game_id = 2;
 * @return {string}
 */
proto.larp.AccountAdmin.prototype.getGameId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.AccountAdmin} returns this
 */
proto.larp.AccountAdmin.prototype.setGameId = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional AdminRank rank = 3;
 * @return {!proto.larp.AdminRank}
 */
proto.larp.AccountAdmin.prototype.getRank = function() {
  return /** @type {!proto.larp.AdminRank} */ (jspb.Message.getFieldWithDefault(this, 3, 0));
};


/**
 * @param {!proto.larp.AdminRank} value
 * @return {!proto.larp.AccountAdmin} returns this
 */
proto.larp.AccountAdmin.prototype.setRank = function(value) {
  return jspb.Message.setProto3EnumField(this, 3, value);
};


/**
 * @enum {number}
 */
proto.larp.AdminRank = {
  ADMIN_RANK_NOT_ADMIN: 0,
  ADMIN_RANK_ADMIN: 1
};

goog.object.extend(exports, proto.larp);
