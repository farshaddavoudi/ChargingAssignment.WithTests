namespace CharginAssignment.WithTests.Web.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected const string JsonContentType = "application/json";
    protected const string BusinessLogicDescription = "Business Logic Error";
    protected const string FetchedDescription = "Fetched Successfully";
    protected const string DeletedDescription = "Deleted Successfully";
    protected const string UpdatedDescription = "Updated Successfully";
    protected const string CreatedDescription = "Created Successfully";

    [NonAction]
    protected IActionResult Created(object value)
    {
        return StatusCode(StatusCodes.Status201Created, value);
    }

    [NonAction]
    protected new IActionResult Created()
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}