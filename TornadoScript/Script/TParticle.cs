using System;
using GTA;
using GTA.Native;
using GTA.Math;
using ScriptCore;

namespace TornadoScript.Script
{
    /// <summary>
    /// Represents a particle in the tornado.
    /// </summary>
    public sealed class Particle : ScriptProp
    {
        public int LayerIndex { get; }

        public Vortex Parent { get; set; }

        private Vector3 _centerPos;

        private readonly Quaternion _rotation;

        private readonly LoopedParticle _ptfx;

        private readonly float _xRadius, _yRadius;

        private float _angle, _layerMask;

        /// <summary>
        /// Instantiate the class.
        /// </summary>
        /// <param name="vortex"></param>
        /// <param name="fxAsset"></param>
        /// <param name="fxName"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        /// <param name="layerIdx"></param>
        public Particle(Vortex vortex, Vector3 position, Vector3 angle, string fxAsset, string fxName, float radiusX, float radiusY, int layerIdx) 
            : base(Setup(position))
        {   
            Parent = vortex;      
            _centerPos = position;
            _rotation = MathEx.Euler(angle);
            _ptfx = new LoopedParticle(fxAsset, fxName);
            _xRadius = radiusX;
            _yRadius = radiusY;
            LayerIndex = layerIdx;
            PostSetup();
        }

        private void PostSetup()
        {
            _layerMask = 1.0f - ((float)LayerIndex / Parent.MaxLayers);

            _layerMask *= (0.1f * LayerIndex);

            _layerMask = 1.0f - _layerMask;

            if (_layerMask < 0.30f)
                _layerMask = 0.30f;
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
            if (Parent == null)
            {
                Dispose();
            }

            else
            {
                _centerPos = Parent.Position + new Vector3(0, 0, Parent.LayerSize * LayerIndex);

                if (Math.Abs(_angle) > Math.PI * 2.0f)
                {
                    _angle = 0.0f;
                }

                Ref.Position = _centerPos + MathEx.MultiplyVector(new Vector3(_xRadius * (float) Math.Cos(_angle), _yRadius * (float) Math.Sin(_angle), 0), _rotation);

                _angle -= (Parent.Speed * _layerMask) * Function.Call<float>(Hash.GET_FRAME_TIME);
            }

            base.OnUpdate(gameTime);
        }

        public void StartFx(float scale)
        {
            if (!_ptfx.IsLoaded)
            {
                _ptfx.Load();
            }

            _ptfx.Start(this, scale);

            _ptfx.Colour = System.Drawing.Color.Black;
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