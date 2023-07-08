using System;
using System.Threading.Tasks;
using AutoMapper;
using HomeApi.Contracts.Models.Devices;
using HomeApi.Contracts.Models.Rooms;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using HomeApi.Data.Repos;
using Microsoft.AspNetCore.Mvc;

namespace HomeApi.Controllers
{
    /// <summary>
    /// Контроллер комнат
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : ControllerBase
    {
        private IRoomRepository _repository;
        private IRoomRepository _rooms;
        private IMapper _mapper;
        
        public RoomsController(IRoomRepository repository, IRoomRepository rooms, IMapper mapper)
        {
            _repository = repository;
            _rooms = rooms;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Обновление существующей комнаты
        /// </summary>
        [HttpPut] 
        [Route("")] 
        public async Task<IActionResult> Edit(
            [FromBody]  EditRoomRequest request)
        {
            var room = await _rooms.GetRoomByName(request.Name);
            if(room == null)
                return StatusCode(400, $"Ошибка: Комната {request.Name} отсутствует, поэтому нечего редактировать.");

            var withSameName = await _repository.GetRoomByName(request.NewName);
            if(withSameName != null)
                return StatusCode(400, $"Ошибка: Комната с именем {request.NewName} уже существует. Выберите другое имя!");

            await _rooms.UpdateRoom(
                room,
                new UpdateRoomQuery(request.NewName, request.NewArea, request.NewGasConnected, request.NewVoltage)
            );

            return StatusCode(200, $"Комната обновлена! Новые данные:" +
                                   $"Имя - {room.Name}, " +
                                   $"Площадь - {room.Area},  " +
                                   $"Газ - {room.GasConnected}, " +
                                   $"Электричество - {room.Voltage}");
        }
        
        //TODO: Задание - добавить метод на получение всех существующих комнат
        
        /// <summary>
        /// Добавление комнаты
        /// </summary>
        [HttpPost] 
        [Route("")] 
        public async Task<IActionResult> Add([FromBody] AddRoomRequest request)
        {
            var existingRoom = await _repository.GetRoomByName(request.Name);
            if (existingRoom == null)
            {
                var newRoom = _mapper.Map<AddRoomRequest, Room>(request);
                await _repository.AddRoom(newRoom);
                return StatusCode(201, $"Комната {request.Name} добавлена!");
            }
            
            return StatusCode(409, $"Ошибка: Комната {request.Name} уже существует.");
        }
    }
}