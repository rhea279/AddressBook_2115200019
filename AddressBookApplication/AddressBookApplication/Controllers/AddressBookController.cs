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

            
            var contact = _mapper.Map<AddressBookEntry>(contactDto);
            await _addressBookBL.AddContact(contact);

            
            var createdDto = _mapper.Map<AddressBookDTO>(contact);
            return CreatedAtAction(nameof(GetContactById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookDTO contactDto)
        {
            if (id != contactDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var validationResult = await _validator.ValidateAsync(contactDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var contact = _mapper.Map<AddressBookEntry>(contactDto);
            await _addressBookBL.UpdateContact(contact);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            await _addressBookBL.DeleteContact(id);
            return NoContent();
        }
    }
}
