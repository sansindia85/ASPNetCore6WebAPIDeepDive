using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FluentValidationMVC.Controllers
{
    public class PeopleController : Controller
    {
        private IValidator<Person> _validator;
        private IPersonRepository _repository;

        public PeopleController(IValidator<Person> validator, IPersonRepository repository)
        {
            // Inject our validator and also a DB context for storing our person object.
            _validator = validator;
            _repository = repository;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            ValidationResult result = await _validator.ValidateAsync(person);

            if (!result.IsValid)
            {
                // Copy the validation results into ModelState.
                // ASP.NET uses the ModelState collection to populate 
                // error messages in the View.
                result.AddToModelState(this.ModelState);

                // re-render the view when validation failed.
                return View("Create", person);
            }

            _repository.Save(person); //Save the person to the database, or some other logic

            TempData["notice"] = "Person successfully created";
            return RedirectToAction("Index");
        }
    }

    internal interface IPersonRepository
    {
    }

    public static class Extensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                
            }
        }
    }
}
