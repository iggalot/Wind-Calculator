using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public Vector3 WorldUp { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);  // The global up direction.


        /// <summary>
        /// Camera Attributes
        /// </summary>
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 CameraUp { get; set; } = new Vector3();
        public Vector3 CameraRight { get; set; } = new Vector3();
        public Vector3 CameraFront { get; set; } = new Vector3();


        public Matrix4x4 ViewMatrix {
            get 
            {
                return m_mViewMatrix;
            }
            set
            {
                m_mViewMatrix = value;
            }
        }
        public Matrix4x4 ModelMatrix { get; set; }
        public Matrix4x4 ProjectionMatrix { get; set; }

        
        /// <summary>
        /// Camera options
        /// </summary>
        public Vector3 OriginalPosition { get; set; }

        private float m_deltaTime = 0.0f;
        private float m_lastFrameTime = 0.0f;

        // Constructor with vectors
        public Camera(Vector3 position, Vector3 up, float yaw = DEFAULT_CAMERA_YAW, float pitch = DEFAULT_CAMERA_PITCH)
        {
            OriginalPosition = position;
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            CameraFront = Vector3.Subtract(Position, Target).Normalize();
            CameraRight = WorldUp.Cross(CameraFront).Normalize();


            UpdateCameraVectors();
            setCameraState(false);

            ModelMatrix = Matrix4x4.Identity;
            ViewMatrix = Matrix4x4.Identity;
            ProjectionMatrix = Matrix4x4.Identity;
        }

        // Constructor with scalar values
        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Position = new Vector3(posX, posY, posZ);
            OriginalPosition = Position;
            WorldUp = new Vector3(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;

            CameraFront = Vector3.Subtract(Position, Target).Normalize();
            CameraRight = WorldUp.Cross(CameraFront).Normalize();
            
            UpdateCameraVectors();
            setCameraState(false);

            ModelMatrix = Matrix4x4.Identity;
            ViewMatrix = Matrix4x4.Identity;
            ProjectionMatrix = Matrix4x4.Identity;
        }

        // Processes input received from any keyboard-like input system. Accepts input parameter in the form of camera defined ENUM (to abstract it from windowing systems)
        void ProcessKeyboard(CameraMovementDirections direction, float deltaTime) {
            float velocity = MovementSpeed * m_deltaTime;
            if (direction == CameraMovementDirections.FORWARD)
                Position += CameraFront * velocity;
            if (direction == CameraMovementDirections.BACKWARD)
                Position -= CameraFront * velocity;
            if (direction == CameraMovementDirections.LEFT)
                Position -= CameraRight * velocity;
            if (direction == CameraMovementDirections.RIGHT)
                Position += CameraRight * velocity;
            if (direction == CameraMovementDirections.UP)
                Position += CameraUp * velocity;
            if (direction == CameraMovementDirections.DOWN)
                Position -= CameraUp * velocity;
        }

        // Process input received from a mouse input system.  Expects the offset value in both the x and y direction
        void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
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
            Position = OriginalPosition;
            CameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
            Yaw = DEFAULT_CAMERA_YAW;
            Pitch = DEFAULT_CAMERA_PITCH;
            //this->updateCameraVectors();

            ModelMatrix = Matrix4x4.Identity;        //default identity matrix
            ViewMatrix = Matrix4x4.Identity;         //default identity matrix
            ProjectionMatrix = Matrix4x4.Identity;   //default identity matrix  
        }

        void setCameraState(bool state)
        {
            m_bCameraIsActive = state;
            return;
        }

        bool getCameraState() { return m_bCameraIsActive; }

        void setPos(Vector3 pos)
        {
            Position = pos;
            return;
        }

        Vector3 getPos() { return Position; }

        void UpdateCameraVectors()
        {
            // Calculate the new Front vector
            Vector3 front;
            front.X = (float)(Math.Cos(Yaw.ToRadians()) * Math.Cos(Pitch.ToRadians()));
            front.Y = (float)(Math.Sin(Pitch.ToRadians()));
            front.Z = (float)(Math.Sin(Yaw.ToRadians()) * Math.Cos(Pitch.ToRadians()));
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
        public static Matrix4x4 LookAt(Vector3 camera_pos, Vector3 target, Vector3 world_up)
        {
            Vector3 front = (target - camera_pos).Normalize();
            Vector3 right = front.Cross(world_up).Normalize();
            Vector3 up = right.Cross(front);
            Matrix4x4 m = Matrix4x4.Identity;
            m.M11 = right.X;
            m.M12 = right.Y;
            m.M13 = right.Z;

            m.M21 = up.X;
            m.M22 = up.Y;
            m.M23 = up.Z;
            
            m.M31 = -front.X;
            m.M32 = -front.Y;
            m.M33 = -front.Z;

            Matrix4x4 m2 = Matrix4x4.Identity;
            m2.M14 = -right.Dot(camera_pos);
            m2.M24 = -up.Dot(camera_pos);
            //m2.M43 = -front.Dot(camera_pos);
            m2.M34 = 22222222222;
            return Matrix4x4.Multiply(m, m2);
        }

        /// <summary>
        /// Creates a matrix for a symmetric perspective-view frustum with far plane at infinite.
        /// </summary>
        public static Matrix4x4 InfinitePerspective(float fovy, float aspect, float zNear)
        {
            float range = (float)(Math.Tan(fovy / 2.0) * zNear);
            float l = -range * (float)aspect;
            float r = range * (float)aspect;
            float b = -range;
            float t = range;
            Matrix4x4 m = Matrix4x4.Identity;
            m.M11 = (float)(((2.0) * zNear) / (r - l));
            m.M22 = (float)(((2.0) * zNear) / (t - b));
            m.M33 = (float)(-(1.0));
            m.M34 = (float)(-(1.0));
            m.M43 = (float)(-(2.0) * zNear);
            return m;
        }

        public static Matrix4x4 FrustumProjection(float left, float right, float bottom, float top, float nearVal, float farVal)
        {
            Matrix4x4 m = Matrix4x4.Identity;
            m.M11 = (2 * nearVal) / (right - left);
            m.M22 = (2 * nearVal) / (top - bottom);
            m.M31 = (right + left) / (right - left);
            m.M32 = (top + bottom) / (top - bottom);
            m.M33 = -(farVal + nearVal) / (farVal - nearVal);
            m.M34 = -1;
            m.M43 = -(2 * farVal * nearVal) / (farVal - nearVal);

            return m;
        }

        /// <summary>
        /// Creates a matrix for an orthographic parallel viewing volume.
        /// </summary>
        public static Matrix4x4 OrthoClipped(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            Matrix4x4 m = Matrix4x4.Identity;
            m.M11 = 2 / (right - left);
            m.M22 = 2 / (top - bottom);
            m.M33 = -2 / (zFar - zNear);
            m.M41 = -(right + left) / (right - left);
            m.M42 = -(top + bottom) / (top - bottom);
            m.M43 = -(zFar + zNear) / (zFar - zNear);
            return m;
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
        public static Matrix4x4 Perspective(float fovy, float aspect, float zNear, float zFar)
        {
            float tanHalfFovy = (float)(Math.Tan(fovy.ToRadians() / 2.0));
            Matrix4x4 m = new Matrix4x4();
            m.M11 = (float)(1 / (aspect * tanHalfFovy));
            m.M22 = (float)(1 / (tanHalfFovy));
            m.M33 = (float)(-(zFar + zNear) / (zFar - zNear));
            m.M34 = (float)(-1);
            m.M43 = (float)(-2 * zFar * zNear) / (zFar - zNear);
            return m;
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a field of view.
        /// </summary>
        public static Matrix4x4 PerspectiveFov(float fov, float width, float height, float zNear, float zFar)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException("width");
            if (height <= 0) throw new ArgumentOutOfRangeException("height");
            if (fov <= 0) throw new ArgumentOutOfRangeException("fov");
            float h = (float)(Math.Cos((double)fov / 2.0) / Math.Sin((double)fov / 2.0));
            float w = h * (height / width);
            Matrix4x4 m = new Matrix4x4();
            m.M11 = w;
            m.M22 = h;
            m.M33 = -(zFar + zNear) / (zFar - zNear);
            m.M34 = -1;
            m.M43 = -(2 * zFar * zNear) / (zFar - zNear);
            return m;
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
        public static Vector4 WorldToScreen(Vector4 point3D, Matrix4x4 view_matrix, Matrix4x4 projection_matrix, double width, double height)
        {
            Console.WriteLine(view_matrix.ToString());
            Console.WriteLine(projection_matrix.ToString());

            Matrix4x4 viewProjectionMatrix = Matrix4x4.Multiply(projection_matrix, view_matrix);
            Vector4 new_point;

            // transform worl to clipping coords
            new_point = viewProjectionMatrix.MatrixVectorProduct(new Vector4(point3D.X, point3D.Y, point3D.Z, 1.0f));

            float winX = (float)Math.Round(((new_point.X + 1) / 2.0) * width);
            // Calcuate the -Y because Y axis is oriented top->down
            float winY = (float)Math.Round(((1 - new_point.Y) / 2.0) * height);

            return new Vector4(winX, winY, 0.0f, 1.0f);
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
