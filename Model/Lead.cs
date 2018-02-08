/*
' Copyright (c) 2018 Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;

namespace Christoc.Modules.FacebookLeads.Model
{
   public class Lead

    {

        ///<summary>

        /// The ID of your object with the name of the ItemName

        ///</summary>

        public int LeadId { get; set; } = -1;

        ///<summary>

        /// A string with the name of the FirstName

        ///</summary>

        public string FirstName { get; set; }

        
        ///<summary>

        /// A string with the name of the LastName

        ///</summary>

        public string LastName { get; set; }

        
        ///<summary>

        /// A string with the name of the Email

        ///</summary>

        public string Email { get; set; }



        ///<summary>

        /// A string with the name of the CellPhone

        ///</summary>

        public string CellPhone { get; set; }

            
        ///<summary>

        /// The ModuleId of where the object was created and gets displayed

        ///</summary>

        public int ModuleId { get; set; }



        ///<summary>

        /// An integer for the user id of the user who created the object

        ///</summary>

        public int CreatedByUserId { get; set; } = -1;



        ///<summary>

        /// An integer for the user id of the user who last updated the object

        ///</summary>

        public int LastModifiedByUserId { get; set; } = -1;



        ///<summary>

        /// The date the object was created

        ///</summary>

        public DateTime CreatedOnDate { get; set; } = DateTime.UtcNow;



        ///<summary>

        /// The date the object was updated

        ///</summary>

        public DateTime LastModifiedOnDate { get; set; } = DateTime.UtcNow;

    }
}