using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public CharacterService(IMapper mapper, DataContext context)
{
            this.mapper = mapper;
            this.context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = this.mapper.Map<Character>(newCharacter);
            this.context.Add(character);
            await this.context.SaveChangesAsync();
            serviceResponse.Data = await this.context.Characters.Select(c => this.mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            
            try
            {  
                Character character = await this.context.Characters.FirstAsync(c => c.Id == id);
                
                this.context.Characters.Remove(character);
                await this.context.SaveChangesAsync();
                response.Data = this.context.Characters.Select(c => this.mapper.Map<GetCharacterDto>(c)).ToList();
            }  
            catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters= await this.context.Characters.ToListAsync();
            response.Data=dbCharacters.Select(c => this.mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await this.context.Characters.FirstOrDefaultAsync(c=> c.Id == id);
            serviceResponse.Data = this.mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            
            try
            {  
                Character character = await this.context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                
                //this.mapper.Map(updatedCharacter, character);
                character.Name=updatedCharacter.Name;
                character.HitPoints=updatedCharacter.HitPoints;
                character.Strength=updatedCharacter.Strength;
                character.Defense=updatedCharacter.Defense;
                character.Intelligence=updatedCharacter.Intelligence;
                character.Class=updatedCharacter.Class;
                await this.context.SaveChangesAsync();
                response.Data=this.mapper.Map<GetCharacterDto>(character);
            }  
            catch(Exception ex){
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}