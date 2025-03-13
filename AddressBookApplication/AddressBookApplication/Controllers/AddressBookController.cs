using AutoMapper;
using BusinessLayer.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace AddressBookApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;
        private readonly IMapper _mapper;
        private readonly IValidator<AddressBookDTO> _validator;
        public AddressBookController(IAddressBookBL addressBookBL, IMapper mapper, IValidator<AddressBookDTO> validator)
        {
            _addressBookBL = addressBookBL;
            _validator = validator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts= await _addressBookBL.GetAllContacts();
            var contactDtos = _mapper.Map<List<AddressBookDTO>>(contacts);
            return Ok(contactDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _addressBookBL.GetContactById(id);
            if (contact == null)
            {
                return NotFound();
            }
            var contactDto = _mapper.Map<AddressBookDTO>(contact);
            return Ok(contactDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] AddressBookDTO contactDto)
        {
            var validationResult = await _validator.ValidateAsync(contactDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdContact = await _addressBookBL.AddContact(contactDto); // ✅ Pass DTO directly
            if (createdContact == null)
            {
                return StatusCode(500, new { Message = "Error creating contact" }); // Handle errors
            }

            return CreatedAtAction(nameof(GetContactById), new { id = createdContact.Id }, createdContact);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookDTO contactDto)
        {
            if (id != contactDto.Id)
            {
                return BadRequest(new { Message = "ID mismatch" });
            }

            var validationResult = await _validator.ValidateAsync(contactDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            bool isUpdated = await _addressBookBL.UpdateContact(contactDto);

            if (!isUpdated)
            {
                return NotFound(new { Message = "Contact not found" }); 
            }

            return Ok(new { Message = "Contact updated successfully" });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            bool isDeleted = await _addressBookBL.DeleteContact(id);
            if (!isDeleted)
            {
                return NotFound(new { Message = "Contact not found" }); // 404 response
            }
            return Ok(new { Message = "Contact deleted successfully" });
        }

    }
}
