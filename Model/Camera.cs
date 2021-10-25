using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WindCalculator.Model
{
    /// <summary>
    /// Enum for describing camera movememnt direction
    /// </summary>
    public enum CameraMovementDirections
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    /// <summary>
    /// A basic camera class
    /// </summary>
    public class Camera
    {
        const float DEFAULT_CAMERA_YAW = -90.0f;
        const float DEFAULT_CAMERA_PITCH = 0.0f;
        const float DEFAULT_CAMERA_SPEED = 3.0f;
        const float DEFAULT_CAMERA_SENSITIVITY = -0.25f;
        const float DEFAULT_CAMERA_ZOOM = 1.0f;
        const float DEFAULT_CAMERA_FOV = 45.0f;

        private bool m_bCameraIsActive = false;
        private Matrix4x4 m_mViewMatrix = new Matrix4x4();

        // Default camera values
        public float Yaw { get; set; } = DEFAULT_CAMERA_YAW;
        public float Pitch { get; set; } = DEFAULT_CAMERA_PITCH;
        public float MovementSpeed { get; set; } = DEFAULT_CAMERA_SPEED;
        public float MouseSensitivity { get; set; } = DEFAULT_CAMERA_SENSITIVITY;
        public float Zoom { get; set; } = DEFAULT_CAMERA_ZOOM;

        public float FOV { get; set; } = DEFAULT_CAMERA_FOV;

        // Constants
        public Vector4 WorldUp { get; set; } = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);  // The global up direction vector.


        /// <summary>
        /// Camera Attributes
        /// </summary>
        public Vector4 CameraPosition { get; set; } = new Vector4(0.0f, 0.0f, 3.0f, 1.0f); // assumes camera is back from the origina few units
        public Vector4 CameraTarget { get; set; } = new Vector4(0.0f, 0.0f, 0.0f, 1.0f); // assumes target is the origin
        public Vector4 CameraUp { get; set; } = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);  // assumes +Y is up
        public Vector4 CameraRight { get; set; } = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // assumes "right" is +X
        public Vector4 CameraFront { get; set; } = new Vector4(0.0f, 0.0f, -1.0f, 1.0f); // assumes that "front" is into the screen or -Z direction


        public Matrix4x4 ViewMatrix { get; set; }
        public Matrix4x4 ModelMatrix { get; set; }
        public Matrix4x4 ProjectionMatrix { get; set; }

        
        /// <summary>
        /// Camera options
        /// </summary>
        public Vector4 OriginalPosition { get; set; }

        private float m_deltaTime = 0.0f;
        private float m_lastFrameTime = 0.0f;

        public bool IsActive = false;

        // Constructor with vectors
        public Camera(Canvas c, Vector4 position, Vector4 up, float yaw = DEFAULT_CAMERA_YAW, float pitch = DEFAULT_CAMERA_PITCH)
        {
            OriginalPosition = position;
            CameraPosition = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            CameraTarget = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            CameraFront = Vector4.Subtract(CameraTarget, CameraPosition).Normalize();
            CameraRight = CameraFront.Cross(WorldUp).Normalize();
            CameraUp = CameraRight.Cross(CameraFront).Normalize();

            UpdateCameraVectors();

            ModelMatrix = Matrix4x4.Identity;
            ModelMatrix = ModelMatrix.ScaleBy(Zoom);

            ViewMatrix = PointAt(CameraPosition, CameraTarget, WorldUp);
            UpdateViewMatrix();

            //ProjectionMatrix = Camera.Ortho((float)-c.Width/2.0f, (float)c.Width/2.0f, (float)-c.Height/2.0f, (float)c.Height/2.0f);
            ProjectionMatrix = Camera.Perspective(FOV, (float)(c.Width / c.Height), 0.1f, 100.0f);
            //ProjectionMatrix = Matrix4x4.Identity;

        }


        // Constructor with scalar values
        public Camera(Canvas c, float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            CameraPosition = new Vector4(posX, posY, posZ, 1.0f);
            OriginalPosition = CameraPosition;
            WorldUp = new Vector4(upX, upY, upZ, 0.0f);
            Yaw = yaw;
            Pitch = pitch;

            CameraTarget = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            CameraFront = Vector4.Subtract(CameraTarget, CameraPosition).Normalize();
            CameraRight = CameraFront.Cross(WorldUp).Normalize();
            CameraUp = CameraRight.Cross(CameraFront).Normalize();

            ViewMatrix = PointAt(CameraPosition, CameraTarget, WorldUp);
            UpdateCameraVectors();

            ModelMatrix = Matrix4x4.Identity; 
            ModelMatrix = ModelMatrix.ScaleBy(Zoom);

            UpdateViewMatrix();

            // Ortho projection does not work
            //ProjectionMatrix = Camera.Ortho((float)-c.Width/2.0f, (float)c.Width/2.0f, (float)-c.Height/2.0f, (float)c.Height/2.0f);
            ProjectionMatrix = Camera.Perspective(FOV, (float)(c.Width / c.Height), 0.1f, 100.0f);
            //ProjectionMatrix = Matrix4x4.Identity;
        }

        public void Update()
        {
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = PointAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
            CameraFront = (new Vector4(ViewMatrix.M31, ViewMatrix.M32, ViewMatrix.M33, 0.0f)).Normalize();
            CameraRight = (new Vector4(ViewMatrix.M11, ViewMatrix.M12, ViewMatrix.M13, 0.0f)).Normalize();
            CameraUp = (new Vector4(ViewMatrix.M21, ViewMatrix.M22, ViewMatrix.M23, 0.0f)).Normalize();

        }

        // Processes input received from any keyboard-like input system. Accepts input parameter in the form of camera defined ENUM (to abstract it from windowing systems)
        public void ProcessKeyboard(CameraMovementDirections direction, float deltaTime) {
            float velocity = MovementSpeed * deltaTime;
            if (direction == CameraMovementDirections.FORWARD)
                CameraPosition += CameraFront * velocity;
            if (direction == CameraMovementDirections.BACKWARD)
                CameraPosition -= CameraFront * velocity;
            if (direction == CameraMovementDirections.LEFT)
                CameraPosition -= CameraRight * velocity;
            if (direction == CameraMovementDirections.RIGHT)
                CameraPosition += CameraRight * velocity;
            if (direction == CameraMovementDirections.UP)
                CameraPosition += CameraUp * velocity;
            if (direction == CameraMovementDirections.DOWN)
                CameraPosition -= CameraUp * velocity;

            UpdateCameraVectors();

        }

        // Process input received from a mouse input system.  Expects the offset value in both the x and y direction
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            // If the camera is turned off, terminate the update events
            if (IsActive == false)
                return;

            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            // Make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            // Update Front, Right and Up Vectors using the updated Eular angles
            UpdateCameraVectors();
        }

        // resets the camera view back to the original position
        void ResetCameraView()
        {
            CameraPosition = OriginalPosition;
            CameraUp = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            CameraFront = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);
            Yaw = DEFAULT_CAMERA_YAW;
            Pitch = DEFAULT_CAMERA_PITCH;
            //this->updateCameraVectors();

            ModelMatrix = Matrix4x4.Identity;        //default identity matrix
            ViewMatrix = Matrix4x4.Identity;         //default identity matrix
            ProjectionMatrix = Matrix4x4.Identity;   //default identity matrix  
        }

        bool getCameraState() { return m_bCameraIsActive; }

        void setPos(Vector4 pos)
        {
            CameraPosition = pos;
            return;
        }

        Vector4 getPos() { return CameraPosition; }

        void UpdateCameraVectors()
        {
            //// Calculate the new Front vector
            Vector4 front;
            front.X = (float)(Math.Cos(Yaw.ToRadians()) * Math.Cos(Pitch.ToRadians()));
            front.Y = (float)(Math.Sin(Pitch.ToRadians()));
            front.Z = (float)(Math.Sin(Yaw.ToRadians()) * Math.Cos(Pitch.ToRadians()));
            front.W = 0.0f;
            CameraFront = front.Normalize();
            // Also re-calculate the Right and Up vector
            CameraRight = CameraFront.Cross(WorldUp).Normalize(); // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            CameraUp = CameraRight.Cross(CameraFront).Normalize();
        }

        /// <summary>
        /// Builds a look at view matrix
        /// </summary>
        /// <param name="eye">the vector for the position of the eye</param>
        /// <param name="center">the vector to the point being looked at</param>
        /// <param name="world_up">the up vector for the world</param>
        /// <returns></returns>
        public static Matrix4x4 PointAt(Vector4 camera_pos, Vector4 target, Vector4 world_up)
        {
            // calculate the new forward direction
            Vector4 newForward = (target - camera_pos).Normalize();

            // calculate the new up
            Vector4 a = Vector4.Multiply(newForward, world_up.Dot(newForward));
            Vector4 newUp = Vector4.Subtract(world_up, a).Normalize();

            // calculate the new right
            Vector4 newRight = newUp.Cross(newForward).Normalize();

            Matrix4x4 m = new Matrix4x4();
            m.M11 = newRight.X;
            m.M12 = newRight.Y;
            m.M13 = newRight.Z;
            m.M14 = 0.0f;

            m.M21 = newUp.X;
            m.M22 = newUp.Y;
            m.M23 = newUp.Z;
            m.M24 = 0.0f;

            m.M31 = newForward.X;
            m.M32 = newForward.Y;
            m.M33 = newForward.Z;
            m.M34 = 0.0f;

            m.M41 = camera_pos.X;
            m.M42 = camera_pos.Y;
            m.M43 = camera_pos.Z;
            m.M44 = 1.0f;

            Matrix4x4 result;
            if (Matrix4x4.Invert(m, out result))
                return result;
            throw new InvalidOperationException("Error inverting matrix in LookAt function");
        }


        /// <summary>
        /// Creates a matrix for projecting two-dimensional coordinates onto the screen.
        /// </summary>
        public static Matrix4x4 Ortho(float left, float right, float bottom, float top)
        {
            Matrix4x4 m = Matrix4x4.Identity;
            m.M11 = 2 / (right - left);
            m.M22 = 2 / (top - bottom);
            m.M33 = -1;
            m.M41 = -(right + left) / (right - left);
            m.M42 = -(top + bottom) / (top - bottom);
            return m;
        }

        /// <summary>
        /// Creates a perspective transformation matrix.
        /// </summary>
        public static Matrix4x4 Perspective(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fCotHalfFovyRad = 1.0f / ((float)(Math.Tan(fFovDegrees.ToRadians() / 2.0)));
            Matrix4x4 m = new Matrix4x4();
            m.M11 = (float)fAspectRatio * fCotHalfFovyRad;
            m.M22 = (float)(fCotHalfFovyRad);
            m.M33 = (float)(fFar / (fFar - fNear));
            m.M43 = (float)(1);
            m.M34 = (float)(- fFar * fNear) / (fFar - fNear);
            m.M44 = 0.0f;

            return Matrix4x4.Transpose(m);
        }

        /// <summary>
        /// Map the specified object coordinates (obj.x, obj.y, obj.z) into window coordinates.
        /// </summary>
        public static Vector4 Project(Vector3 obj, Matrix4x4 model, Matrix4x4 proj, Matrix4x4 view)
        {
            Vector4 tmp = proj.MatrixVectorProduct(view.MatrixVectorProduct(model.MatrixVectorProduct(new Vector4(obj.X, obj.Y, obj.Z, 1.0f))));

            //tmp /= tmp.W;
            //tmp = Vector4.Add(Vector4.Multiply(tmp, 0.5f) , new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
            //tmp.X = tmp.X * viewport.Z + viewport.X;
            //tmp.Y = tmp.Y * viewport.W + viewport.Y;
            return tmp;
        }

        /// <summary>
        /// Uses a projection matrix to  convert a 3D point in world space to a 2D point on the screen
        /// </summary>
        /// <param name="point3D"></param>
        /// <param name="view_matrix"></param>
        /// <param name="projection_matrix"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vector4 WorldToScreen(Vector4 point3D, Matrix4x4 model_matrix, Matrix4x4 projection_matrix, Matrix4x4 view_matrix, double width, double height)
        {
            // Coordinates are assumed to be in world space

            //Console.WriteLine(view_matrix.ToString());
            //Console.WriteLine(projection_matrix.ToString());

            // Move and scale the model
            Vector4 newPoint = model_matrix.MatrixVectorProduct(point3D);

            // Transform into view
            Vector4 t1 = view_matrix.MatrixVectorProduct(newPoint);

            // Project onto screen
            t1 = projection_matrix.MatrixVectorProduct(t1);

            // Project
            t1.X = t1.X / t1.W;
            t1.Y = t1.Y / t1.W;
            t1.Z = t1.Z / t1.W;
            t1.W = t1.W / t1.W;

            Vector4 vOffSetView = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

            t1 = Vector4.Add(t1, vOffSetView);

            t1.X *= (float)(0.5f * width);
            t1.Y *= (float)(0.5f * height);

            vOffSetView = new Vector4((float)width, (float)height, 0.0f, 0.0f);
            t1 = Vector4.Add(t1, vOffSetView);

            return t1;

            //Matrix4x4 viewProjectionMatrix = Matrix4x4.Multiply(projection_matrix, view_matrix );

            //// Camera coordinate - view space
            //Vector4 view_coord = viewProjectionMatrix.MatrixVectorProduct(new_point);

            //// Coords of the screen in camera
            ////Vector4 screen_coord = new Vector4(view_coord.X / view_coord.Z, view_coord.Y / view_coord.Z, 0.0f, 1.0f);


            ////// transform world to normalized clipping coords (raster coords)
            ////Vector4 clip_coord = new Vector4((float)((screen_coord.X + width / 2.0) / width), (float)((screen_coord.Y + height / 2.0) / height), 0.0f, 1.0f);
            ////Vector4 clip_coord = new Vector4((float)((screen_coord.X) * width), (float)((screen_coord.Y) * height), 0.0f, 1.0f);

            ////// Invert the y coordinate
            //Vector4 canvas_coord = new Vector4((float)(Math.Round(clip_coord.X)), (float)(Math.Round((1 - clip_coord.Y / height) * height / 2.0f)), 0.0f, 1.0f);
            //////float winX = (float)Math.Round(((screen_coord.X + width / 2) * width);
            ////// Calcuate the -Y because Y axis is oriented top->down
            //////float winY = (float)Math.Round(((1 - view_coord.Y) / 2.0) * height);

            //return canvas_coord;
        }

        /// <summary>
        /// Uses the inverted projection matrux to convert a 2D point on the screen back to the 3D point in space.
        /// </summary>
        /// <param name="screenPt"></param>
        /// <param name="view_matrix"></param>
        /// <param name="projection_matrix"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vector4 ScreenToWorld(Vector4 screenPt, Matrix4x4 view_matrix, Matrix4x4 projection_matrix, double width, double height)
        {
            float x = (float)(2.0 * screenPt.X / width - 1.0f);
            float y = (float)(-2.0 * screenPt.Y / height + 1.0f);
            Vector4 new_point = new Vector4(x, y, 0.0f, 1.0f);

            Matrix4x4 result = new Matrix4x4();

            // Attempt to invert the matrix
            if(!Matrix4x4.Invert(Matrix4x4.Multiply(projection_matrix, view_matrix), out result))
            {
                throw new InvalidOperationException("Unable to invert the view projection matrix in ScreentoWorld()");
            }

            Matrix4x4 viewProjectionInverse = result;

            // transform world to clipping coords
            return viewProjectionInverse.MatrixVectorProduct(new Vector4(new_point.X, new_point.Y, new_point.Z, 1.0f));
        }
    }
}
