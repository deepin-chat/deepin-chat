using Deepin.Application.Commands.Files;
using Deepin.Application.Models.Files;
using Deepin.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Deepin.Application.Services;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class FilesController(
    IMediator mediator,
    IFileQueries fileQueries,
    IFileService fileService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IFileQueries _fileQueries = fileQueries;
    private readonly IFileService _fileService = fileService;
    [HttpPost]
    public async Task<ActionResult<FileModel>> Post(IFormFile file)
    {
        var command = new UploadFileCommand(file);
        var model = await _mediator.Send(command);
        return Ok(model);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<FileModel>> Get(string id)
    {
        var model = await _fileQueries.GetById(id);
        return Ok(model);
    }
    [HttpGet("{id}/download")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult<FileModel>> Download(string id)
    {
        var file = await _fileQueries.GetById(id);
        var stream = await _mediator.Send(new ReadFileCommand(file));
        return File(stream, MimeTypesMap.GetMimeType(file.Name), file.Name);
    }
    [AllowAnonymous]
    [HttpGet("{id}/temporary")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult<FileModel>> TemporaryDownload(string id, string token, long expires)
    {
        var isValidToken = _fileService.ValidateToken(id, token, expires);
        if (!isValidToken)
        {
            return Forbid();
        }
        var file = await _fileQueries.GetById(id);
        if (file == null)
            return NotFound();
        var stream = await _mediator.Send(new ReadFileCommand(file));
        return File(stream, MimeTypesMap.GetMimeType(file.Name), file.Name);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteFileCommand(id));
        return NoContent();
    }
}
