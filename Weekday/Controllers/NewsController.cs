using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weekday.Data.Interfaces;
using Weekday.Data.Models;
using Weekday.DataContracts;
using Weekday.Miscellaneous;

namespace Weekday.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public NewsController(IMapper mapper, IUnitOfWork unitOfWork, ILogger<NewsController> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("{id}/{pageNumber:int}/{pageSize:int}")]
        public async Task<IReadOnlyCollection<NewsDataContract>> Get(string authorId, int pageNumber, int pageSize)
        {
            var news = await _unitOfWork.News.GetNewsAsync(authorId, pageNumber, pageSize);
            return news.Select(x => _mapper.Map<NewsDataContract>(x)).ToList();
        }

        [HttpPost]
        public ActionResult Create([FromBody] NewsDataContract newsDataContract)
        {
            try
            {
                _unitOfWork.News.Add(_mapper.Map<News>(newsDataContract));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.NewsCreatingFailed, ex, LoggingEvents.NewsCreatingFailed.Name);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public ActionResult Edit([FromBody] NewsDataContract newsDataContract)
        {
            try
            {
                _unitOfWork.News.Update(_mapper.Map<News>(newsDataContract));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.NewsEditingFailed, ex, LoggingEvents.NewsEditingFailed.Name);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        public ActionResult Delete(int id)
        {
            var entity = _unitOfWork.News.Find(x => x.Id == id);
            if (entity != null)
            {
                _unitOfWork.News.Remove(_mapper.Map<News>(entity));
                return Ok();
            }

            return BadRequest();
        }
    }
}
