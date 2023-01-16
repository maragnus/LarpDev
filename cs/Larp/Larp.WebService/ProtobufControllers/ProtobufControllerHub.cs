﻿using System.Collections.Concurrent;
using System.Net;
using Google.Protobuf;

namespace Larp.WebService.ProtobufControllers;

public class ProtobufControllerHub
{
    private const string ProtobufContentType = "application/x-protobuf";
    private readonly ProtobufControllerMap _map;
    private readonly ConcurrentDictionary<Type, Func<Stream, IMessage>> _parsers = new();

    public ProtobufControllerHub(ProtobufControllerMap map)
    {
        _map = map;
    }

    private async Task<object> ReadRequestMessage(Type type, HttpRequest request)
    {
        if (!_parsers.TryGetValue(type, out var parse))
            parse = FindParserMethod(type);

        var memory = new MemoryStream((int)(request.ContentLength ?? 512));
        await request.Body.CopyToAsync(memory);
        memory.Seek(0, SeekOrigin.Begin);
        return parse(memory);
    }

    private Func<Stream, IMessage> FindParserMethod(Type messageType)
    {
        var parserProperty = messageType.GetProperty("Parser")!;
        var parser = parserProperty.GetValue(parserProperty)!;
        var method = parserProperty.PropertyType.GetMethod("ParseFrom", new[] { typeof(Stream) })!;
        IMessage Parse(Stream s) => (IMessage)method.Invoke(parser, new[] { (object)s })!;
        _parsers.TryAdd(messageType, Parse);
        return Parse;
    }

    private async Task WriteResponseMessage(IMessage response, HttpContext context)
    {
        context.Response.ContentType = ProtobufContentType;
        await context.Response.Body.WriteAsync(response.ToByteArray());
    }

    public async Task HandleRequest(string service, string method, HttpContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Find the Controller and Method
        var info = _map.Get(service, method);
        if (info == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        var logger =
            (ILogger)serviceProvider.GetRequiredService(typeof(ILogger<>).MakeGenericType(info.ControllerType));

        // Instantiate the Controller
        var controller = (ProtobufController)serviceProvider.GetRequiredService(info.ControllerType);

        controller.LoggerInternal = logger;
        controller.HttpContextInternal = context;

        {
            var beforeRequestHandler = new RequestHandler(context);
            await controller.BeforeRequest(beforeRequestHandler);
            if (beforeRequestHandler.IsRequestCompleted)
                return;
        }

        var parameters = info.HasRequestMessage
            ? new[] { await ReadRequestMessage(info.RequestMessageType!, context.Request) }
            : Array.Empty<object>();

        dynamic? response;

        // Invoke the Method on the Controller
        if (info.IsAsync)
        {
            dynamic task = info.MethodInfo.Invoke(controller, parameters)!;
            response = await task;
        }
        else
        {
            response = info.MethodInfo.Invoke(controller, parameters);
        }

        {
            var beforeResponseHandler = new RequestHandler(context);
            await controller.BeforeResponse(beforeResponseHandler);
            if (beforeResponseHandler.IsRequestCompleted)
                return;
        }

        // Write the response
        context.Response.StatusCode = info.HasResponseMessage
            ? (int)HttpStatusCode.OK
            : (int)HttpStatusCode.NoContent;

        if (info.HasResponseMessage)
            await WriteResponseMessage(response, context);

        await controller.AfterResponse(context);
    }
}