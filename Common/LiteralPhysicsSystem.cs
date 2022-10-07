using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LiteralBuffMod.Common.LiteralPhysicsUtil;

namespace LiteralBuffMod.Common
{
    public class LiteralPhysicsSystem : ModSystem
    {
        public static List<Entity> magneticEntity = new List<Entity> { };
        public override void PreUpdateEntities()
        {
        }
        public override void PostUpdateEverything()
        {
            if (magneticEntity.Count > 1)
            {
                for (int mE = 0; mE < magneticEntity.Count; mE++)
                {
                    Entity entity = magneticEntity[mE];
                    if (entity == null || !entity.active)
                    {
                        magneticEntity.Remove(entity);
                        continue;
                    }

                    else if (!(entity is Item) && !(entity is Player) && !(entity is NPC) && !(entity is Projectile))
                    {
                        magneticEntity.Remove(entity);
                        continue;
                    }

                    else
                    {
                        Entity[] restMagneticEntity = magneticEntity.Skip(mE + 1).ToArray();
                        for (int rME = 0; rME < restMagneticEntity.Length; rME++)
                        {
                            Entity objEntity = restMagneticEntity[rME];
                            if (objEntity != null && objEntity.active)
                            {
                                entity.velocity += ForceToVelocity(entity, ElectromagneticForce2(entity, objEntity), objEntity.position, objEntity.Size);
                                objEntity.velocity += ForceToVelocity(objEntity, ElectromagneticForce2(objEntity, entity), entity.position, entity.Size);
                                if (entity is Projectile && objEntity is Projectile)
                                {
                                    MagnetSphereTransformProj((Projectile)entity, (Projectile)objEntity);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
