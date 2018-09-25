using System;
using GTA;
using GTA.Math;
using GTA.Native;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Script
{
    /// <summary>
    /// Represents a particle in the tornado.
    /// </summary>
    public sealed class TornadoParticle : ScriptProp
    {
        public int LayerIndex { get; }

        public TornadoVortex Parent { get; set; }

        private Vector3 _centerPos;

        private readonly Vector3 _offset;

        private readonly Quaternion _rotation;

        private readonly LoopedParticle _ptfx;

        private readonly float _radius;

        private float _angle, _layerMask;

        /// <summary>
        /// Instantiate the class.
        /// </summary>
        /// <param name="vortex"></param>
        /// <param name="fxAsset"></param>
        /// <param name="fxName"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <param name="layerIdx"></param>
        public TornadoParticle(TornadoVortex vortex, Vector3 position, Vector3 angle, string fxAsset, string fxName, float radius, int layerIdx) 
            : base(Setup(position))
        {   
            Parent = vortex;      
            _centerPos = position;
            _rotation = MathEx.Euler(angle);
            _ptfx = new LoopedParticle(fxAsset, fxName);
            _radius = radius;          
            _offset = new Vector3(0, 0, ScriptThread.GetVar<float>("vortexLayerSeperationScale") * layerIdx);
            LayerIndex = layerIdx;
            PostSetup();
        }

        private void PostSetup()
        {
            _layerMask = 1.0f - (float)LayerIndex / ScriptThread.GetVar<int>("vortexMaxParticleLayers");

            _layerMask *= 0.1f * LayerIndex;

            _layerMask = 1.0f - _layerMask;

            if (_layerMask <= 0.3f)
               _layerMask = 0.3f;
        }

        /// <summary>
        /// Setup the base entity.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static Prop Setup(Vector3 position)
        {
            var model = new Model("prop_beachball_02");

            if (!model.IsLoaded) model.Request(1000);

            var prop = World.CreateProp(model, position, false, false);

            Function.Call(Hash.SET_ENTITY_COLLISION, prop.Handle, 0, 0);

            prop.IsVisible = false;

            return prop;
        }

        /// <summary>
        /// Set the center position that the particle should rotate around.
        /// </summary>
        /// <param name="center"></param>
        public void SetPosition(Vector3 center)
        {
            _centerPos = center;
        }

        /// <summary>
        /// Set the particle scale.
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            _ptfx.Scale = scale;
        }

        public override void OnUpdate(int gameTime)
        {
           /* if (Parent == null)
            {
                Dispose();
            }

            else
            {*/
                _centerPos = Parent.Position + _offset;

                if (Math.Abs(_angle) > Math.PI * 2.0f)
                {
                    _angle = 0.0f;
                }

                Ref.Position = _centerPos + 
                    MathEx.MultiplyVector(new Vector3(_radius * (float)Math.Cos(_angle), _radius * (float)Math.Sin(_angle), 0), _rotation);

               _angle -= ScriptThread.GetVar<float>("vortexRotationSpeed") * _layerMask * Game.LastFrameTime;
        //    }

            base.OnUpdate(gameTime);
        }

        public void StartFx(float scale)
        {
            if (!_ptfx.IsLoaded)
            {
                _ptfx.Load();
            }

            _ptfx.Start(this, scale);

            _ptfx.Alpha = 0.5f;
        }

        public void RemoveFx()
        {
            _ptfx.Remove();
        }

        public override void Dispose()
        {
            RemoveFx();
            base.Dispose();
        }
    }
}