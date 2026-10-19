
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('family_links', function (Blueprint $table) {
            $table->id();

            $table->foreignId('elder_user_id')
                ->constrained('elder_users')
                ->cascadeOnDelete();

            $table->foreignId('family_user_id')
                ->constrained('family_users')
                ->cascadeOnDelete();

            $table->string('relationship', 50);

            $table->timestamps();

            $table->unique(['elder_user_id', 'family_user_id']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('family_links');
    }
};
