// source: larp/services.proto
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

var larp_accounts_pb = require('../larp/accounts_pb.js');
goog.object.extend(proto, larp_accounts_pb);
var larp_common_pb = require('../larp/common_pb.js');
goog.object.extend(proto, larp_common_pb);
var larp_events_pb = require('../larp/events_pb.js');
goog.object.extend(proto, larp_events_pb);
goog.exportSymbol('proto.larp.services.AccountRequest', null, global);
goog.exportSymbol('proto.larp.services.AccountResponse', null, global);
goog.exportSymbol('proto.larp.services.EventComponentRsvp', null, global);
goog.exportSymbol('proto.larp.services.EventListRequest', null, global);
goog.exportSymbol('proto.larp.services.EventListResponse', null, global);
goog.exportSymbol('proto.larp.services.EventRsvpRequest', null, global);
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
proto.larp.services.AccountResponse = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.services.AccountResponse, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.AccountResponse.displayName = 'proto.larp.services.AccountResponse';
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
proto.larp.services.EventListRequest = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.services.EventListRequest, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.EventListRequest.displayName = 'proto.larp.services.EventListRequest';
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
proto.larp.services.EventListResponse = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.larp.services.EventListResponse.repeatedFields_, null);
};
goog.inherits(proto.larp.services.EventListResponse, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.EventListResponse.displayName = 'proto.larp.services.EventListResponse';
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
proto.larp.services.EventComponentRsvp = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.services.EventComponentRsvp, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.EventComponentRsvp.displayName = 'proto.larp.services.EventComponentRsvp';
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
proto.larp.services.EventRsvpRequest = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, proto.larp.services.EventRsvpRequest.repeatedFields_, null);
};
goog.inherits(proto.larp.services.EventRsvpRequest, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.EventRsvpRequest.displayName = 'proto.larp.services.EventRsvpRequest';
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
proto.larp.services.AccountRequest = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.larp.services.AccountRequest, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.larp.services.AccountRequest.displayName = 'proto.larp.services.AccountRequest';
}



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
proto.larp.services.AccountResponse.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.AccountResponse.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.AccountResponse} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.AccountResponse.toObject = function(includeInstance, msg) {
  var f, obj = {
    account: (f = msg.getAccount()) && larp_accounts_pb.Account.toObject(includeInstance, f)
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
 * @return {!proto.larp.services.AccountResponse}
 */
proto.larp.services.AccountResponse.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.AccountResponse;
  return proto.larp.services.AccountResponse.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.AccountResponse} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.AccountResponse}
 */
proto.larp.services.AccountResponse.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new larp_accounts_pb.Account;
      reader.readMessage(value,larp_accounts_pb.Account.deserializeBinaryFromReader);
      msg.setAccount(value);
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
proto.larp.services.AccountResponse.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.AccountResponse.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.AccountResponse} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.AccountResponse.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAccount();
  if (f != null) {
    writer.writeMessage(
      1,
      f,
      larp_accounts_pb.Account.serializeBinaryToWriter
    );
  }
};


/**
 * optional larp.Account account = 1;
 * @return {?proto.larp.Account}
 */
proto.larp.services.AccountResponse.prototype.getAccount = function() {
  return /** @type{?proto.larp.Account} */ (
    jspb.Message.getWrapperField(this, larp_accounts_pb.Account, 1));
};


/**
 * @param {?proto.larp.Account|undefined} value
 * @return {!proto.larp.services.AccountResponse} returns this
*/
proto.larp.services.AccountResponse.prototype.setAccount = function(value) {
  return jspb.Message.setWrapperField(this, 1, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.larp.services.AccountResponse} returns this
 */
proto.larp.services.AccountResponse.prototype.clearAccount = function() {
  return this.setAccount(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.services.AccountResponse.prototype.hasAccount = function() {
  return jspb.Message.getField(this, 1) != null;
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
proto.larp.services.EventListRequest.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.EventListRequest.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.EventListRequest} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventListRequest.toObject = function(includeInstance, msg) {
  var f, obj = {
    includepast: jspb.Message.getBooleanFieldWithDefault(msg, 1, false),
    includefuture: jspb.Message.getBooleanFieldWithDefault(msg, 2, false),
    includeattendance: jspb.Message.getBooleanFieldWithDefault(msg, 3, false)
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
 * @return {!proto.larp.services.EventListRequest}
 */
proto.larp.services.EventListRequest.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.EventListRequest;
  return proto.larp.services.EventListRequest.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.EventListRequest} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.EventListRequest}
 */
proto.larp.services.EventListRequest.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIncludepast(value);
      break;
    case 2:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIncludefuture(value);
      break;
    case 3:
      var value = /** @type {boolean} */ (reader.readBool());
      msg.setIncludeattendance(value);
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
proto.larp.services.EventListRequest.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.EventListRequest.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.EventListRequest} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventListRequest.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getIncludepast();
  if (f) {
    writer.writeBool(
      1,
      f
    );
  }
  f = message.getIncludefuture();
  if (f) {
    writer.writeBool(
      2,
      f
    );
  }
  f = message.getIncludeattendance();
  if (f) {
    writer.writeBool(
      3,
      f
    );
  }
};


/**
 * optional bool includePast = 1;
 * @return {boolean}
 */
proto.larp.services.EventListRequest.prototype.getIncludepast = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 1, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.services.EventListRequest} returns this
 */
proto.larp.services.EventListRequest.prototype.setIncludepast = function(value) {
  return jspb.Message.setProto3BooleanField(this, 1, value);
};


/**
 * optional bool includeFuture = 2;
 * @return {boolean}
 */
proto.larp.services.EventListRequest.prototype.getIncludefuture = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 2, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.services.EventListRequest} returns this
 */
proto.larp.services.EventListRequest.prototype.setIncludefuture = function(value) {
  return jspb.Message.setProto3BooleanField(this, 2, value);
};


/**
 * optional bool includeAttendance = 3;
 * @return {boolean}
 */
proto.larp.services.EventListRequest.prototype.getIncludeattendance = function() {
  return /** @type {boolean} */ (jspb.Message.getBooleanFieldWithDefault(this, 3, false));
};


/**
 * @param {boolean} value
 * @return {!proto.larp.services.EventListRequest} returns this
 */
proto.larp.services.EventListRequest.prototype.setIncludeattendance = function(value) {
  return jspb.Message.setProto3BooleanField(this, 3, value);
};



/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.larp.services.EventListResponse.repeatedFields_ = [1];



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
proto.larp.services.EventListResponse.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.EventListResponse.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.EventListResponse} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventListResponse.toObject = function(includeInstance, msg) {
  var f, obj = {
    eventList: jspb.Message.toObjectList(msg.getEventList(),
    larp_events_pb.Event.toObject, includeInstance)
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
 * @return {!proto.larp.services.EventListResponse}
 */
proto.larp.services.EventListResponse.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.EventListResponse;
  return proto.larp.services.EventListResponse.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.EventListResponse} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.EventListResponse}
 */
proto.larp.services.EventListResponse.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new larp_events_pb.Event;
      reader.readMessage(value,larp_events_pb.Event.deserializeBinaryFromReader);
      msg.addEvent(value);
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
proto.larp.services.EventListResponse.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.EventListResponse.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.EventListResponse} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventListResponse.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getEventList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      1,
      f,
      larp_events_pb.Event.serializeBinaryToWriter
    );
  }
};


/**
 * repeated larp.Event event = 1;
 * @return {!Array<!proto.larp.Event>}
 */
proto.larp.services.EventListResponse.prototype.getEventList = function() {
  return /** @type{!Array<!proto.larp.Event>} */ (
    jspb.Message.getRepeatedWrapperField(this, larp_events_pb.Event, 1));
};


/**
 * @param {!Array<!proto.larp.Event>} value
 * @return {!proto.larp.services.EventListResponse} returns this
*/
proto.larp.services.EventListResponse.prototype.setEventList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 1, value);
};


/**
 * @param {!proto.larp.Event=} opt_value
 * @param {number=} opt_index
 * @return {!proto.larp.Event}
 */
proto.larp.services.EventListResponse.prototype.addEvent = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 1, opt_value, proto.larp.Event, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.larp.services.EventListResponse} returns this
 */
proto.larp.services.EventListResponse.prototype.clearEventList = function() {
  return this.setEventList([]);
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
proto.larp.services.EventComponentRsvp.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.EventComponentRsvp.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.EventComponentRsvp} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventComponentRsvp.toObject = function(includeInstance, msg) {
  var f, obj = {
    componentName: jspb.Message.getFieldWithDefault(msg, 1, ""),
    type: jspb.Message.getFieldWithDefault(msg, 2, 0),
    characterId: jspb.Message.getFieldWithDefault(msg, 3, "")
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
 * @return {!proto.larp.services.EventComponentRsvp}
 */
proto.larp.services.EventComponentRsvp.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.EventComponentRsvp;
  return proto.larp.services.EventComponentRsvp.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.EventComponentRsvp} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.EventComponentRsvp}
 */
proto.larp.services.EventComponentRsvp.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setComponentName(value);
      break;
    case 2:
      var value = /** @type {!proto.larp.EventAttendanceType} */ (reader.readEnum());
      msg.setType(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setCharacterId(value);
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
proto.larp.services.EventComponentRsvp.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.EventComponentRsvp.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.EventComponentRsvp} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventComponentRsvp.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getComponentName();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getType();
  if (f !== 0.0) {
    writer.writeEnum(
      2,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 3));
  if (f != null) {
    writer.writeString(
      3,
      f
    );
  }
};


/**
 * optional string component_name = 1;
 * @return {string}
 */
proto.larp.services.EventComponentRsvp.prototype.getComponentName = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.services.EventComponentRsvp} returns this
 */
proto.larp.services.EventComponentRsvp.prototype.setComponentName = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional larp.EventAttendanceType type = 2;
 * @return {!proto.larp.EventAttendanceType}
 */
proto.larp.services.EventComponentRsvp.prototype.getType = function() {
  return /** @type {!proto.larp.EventAttendanceType} */ (jspb.Message.getFieldWithDefault(this, 2, 0));
};


/**
 * @param {!proto.larp.EventAttendanceType} value
 * @return {!proto.larp.services.EventComponentRsvp} returns this
 */
proto.larp.services.EventComponentRsvp.prototype.setType = function(value) {
  return jspb.Message.setProto3EnumField(this, 2, value);
};


/**
 * optional string character_id = 3;
 * @return {string}
 */
proto.larp.services.EventComponentRsvp.prototype.getCharacterId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.services.EventComponentRsvp} returns this
 */
proto.larp.services.EventComponentRsvp.prototype.setCharacterId = function(value) {
  return jspb.Message.setField(this, 3, value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.larp.services.EventComponentRsvp} returns this
 */
proto.larp.services.EventComponentRsvp.prototype.clearCharacterId = function() {
  return jspb.Message.setField(this, 3, undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.services.EventComponentRsvp.prototype.hasCharacterId = function() {
  return jspb.Message.getField(this, 3) != null;
};



/**
 * List of repeated fields within this message type.
 * @private {!Array<number>}
 * @const
 */
proto.larp.services.EventRsvpRequest.repeatedFields_ = [4];



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
proto.larp.services.EventRsvpRequest.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.EventRsvpRequest.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.EventRsvpRequest} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventRsvpRequest.toObject = function(includeInstance, msg) {
  var f, obj = {
    eventId: jspb.Message.getFieldWithDefault(msg, 1, ""),
    characterId: jspb.Message.getFieldWithDefault(msg, 2, ""),
    rsvp: jspb.Message.getFieldWithDefault(msg, 3, 0),
    componentsList: jspb.Message.toObjectList(msg.getComponentsList(),
    proto.larp.services.EventComponentRsvp.toObject, includeInstance)
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
 * @return {!proto.larp.services.EventRsvpRequest}
 */
proto.larp.services.EventRsvpRequest.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.EventRsvpRequest;
  return proto.larp.services.EventRsvpRequest.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.EventRsvpRequest} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.EventRsvpRequest}
 */
proto.larp.services.EventRsvpRequest.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setEventId(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setCharacterId(value);
      break;
    case 3:
      var value = /** @type {!proto.larp.EventRsvp} */ (reader.readEnum());
      msg.setRsvp(value);
      break;
    case 4:
      var value = new proto.larp.services.EventComponentRsvp;
      reader.readMessage(value,proto.larp.services.EventComponentRsvp.deserializeBinaryFromReader);
      msg.addComponents(value);
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
proto.larp.services.EventRsvpRequest.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.EventRsvpRequest.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.EventRsvpRequest} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.EventRsvpRequest.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getEventId();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = /** @type {string} */ (jspb.Message.getField(message, 2));
  if (f != null) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getRsvp();
  if (f !== 0.0) {
    writer.writeEnum(
      3,
      f
    );
  }
  f = message.getComponentsList();
  if (f.length > 0) {
    writer.writeRepeatedMessage(
      4,
      f,
      proto.larp.services.EventComponentRsvp.serializeBinaryToWriter
    );
  }
};


/**
 * optional string event_id = 1;
 * @return {string}
 */
proto.larp.services.EventRsvpRequest.prototype.getEventId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.services.EventRsvpRequest} returns this
 */
proto.larp.services.EventRsvpRequest.prototype.setEventId = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string character_id = 2;
 * @return {string}
 */
proto.larp.services.EventRsvpRequest.prototype.getCharacterId = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.larp.services.EventRsvpRequest} returns this
 */
proto.larp.services.EventRsvpRequest.prototype.setCharacterId = function(value) {
  return jspb.Message.setField(this, 2, value);
};


/**
 * Clears the field making it undefined.
 * @return {!proto.larp.services.EventRsvpRequest} returns this
 */
proto.larp.services.EventRsvpRequest.prototype.clearCharacterId = function() {
  return jspb.Message.setField(this, 2, undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.services.EventRsvpRequest.prototype.hasCharacterId = function() {
  return jspb.Message.getField(this, 2) != null;
};


/**
 * optional larp.EventRsvp rsvp = 3;
 * @return {!proto.larp.EventRsvp}
 */
proto.larp.services.EventRsvpRequest.prototype.getRsvp = function() {
  return /** @type {!proto.larp.EventRsvp} */ (jspb.Message.getFieldWithDefault(this, 3, 0));
};


/**
 * @param {!proto.larp.EventRsvp} value
 * @return {!proto.larp.services.EventRsvpRequest} returns this
 */
proto.larp.services.EventRsvpRequest.prototype.setRsvp = function(value) {
  return jspb.Message.setProto3EnumField(this, 3, value);
};


/**
 * repeated EventComponentRsvp components = 4;
 * @return {!Array<!proto.larp.services.EventComponentRsvp>}
 */
proto.larp.services.EventRsvpRequest.prototype.getComponentsList = function() {
  return /** @type{!Array<!proto.larp.services.EventComponentRsvp>} */ (
    jspb.Message.getRepeatedWrapperField(this, proto.larp.services.EventComponentRsvp, 4));
};


/**
 * @param {!Array<!proto.larp.services.EventComponentRsvp>} value
 * @return {!proto.larp.services.EventRsvpRequest} returns this
*/
proto.larp.services.EventRsvpRequest.prototype.setComponentsList = function(value) {
  return jspb.Message.setRepeatedWrapperField(this, 4, value);
};


/**
 * @param {!proto.larp.services.EventComponentRsvp=} opt_value
 * @param {number=} opt_index
 * @return {!proto.larp.services.EventComponentRsvp}
 */
proto.larp.services.EventRsvpRequest.prototype.addComponents = function(opt_value, opt_index) {
  return jspb.Message.addToRepeatedWrapperField(this, 4, opt_value, proto.larp.services.EventComponentRsvp, opt_index);
};


/**
 * Clears the list making it empty but non-null.
 * @return {!proto.larp.services.EventRsvpRequest} returns this
 */
proto.larp.services.EventRsvpRequest.prototype.clearComponentsList = function() {
  return this.setComponentsList([]);
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
proto.larp.services.AccountRequest.prototype.toObject = function(opt_includeInstance) {
  return proto.larp.services.AccountRequest.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.larp.services.AccountRequest} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.AccountRequest.toObject = function(includeInstance, msg) {
  var f, obj = {
    account: (f = msg.getAccount()) && larp_accounts_pb.Account.toObject(includeInstance, f)
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
 * @return {!proto.larp.services.AccountRequest}
 */
proto.larp.services.AccountRequest.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.larp.services.AccountRequest;
  return proto.larp.services.AccountRequest.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.larp.services.AccountRequest} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.larp.services.AccountRequest}
 */
proto.larp.services.AccountRequest.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = new larp_accounts_pb.Account;
      reader.readMessage(value,larp_accounts_pb.Account.deserializeBinaryFromReader);
      msg.setAccount(value);
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
proto.larp.services.AccountRequest.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.larp.services.AccountRequest.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.larp.services.AccountRequest} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.larp.services.AccountRequest.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getAccount();
  if (f != null) {
    writer.writeMessage(
      1,
      f,
      larp_accounts_pb.Account.serializeBinaryToWriter
    );
  }
};


/**
 * optional larp.Account account = 1;
 * @return {?proto.larp.Account}
 */
proto.larp.services.AccountRequest.prototype.getAccount = function() {
  return /** @type{?proto.larp.Account} */ (
    jspb.Message.getWrapperField(this, larp_accounts_pb.Account, 1));
};


/**
 * @param {?proto.larp.Account|undefined} value
 * @return {!proto.larp.services.AccountRequest} returns this
*/
proto.larp.services.AccountRequest.prototype.setAccount = function(value) {
  return jspb.Message.setWrapperField(this, 1, value);
};


/**
 * Clears the message field making it undefined.
 * @return {!proto.larp.services.AccountRequest} returns this
 */
proto.larp.services.AccountRequest.prototype.clearAccount = function() {
  return this.setAccount(undefined);
};


/**
 * Returns whether this field is set.
 * @return {boolean}
 */
proto.larp.services.AccountRequest.prototype.hasAccount = function() {
  return jspb.Message.getField(this, 1) != null;
};


goog.object.extend(exports, proto.larp.services);